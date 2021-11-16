// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.ClientLibrary.Management;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Squidex.CLI.Commands.Implementation.Sync.Schemas
{
    public sealed class SchemasSynchronizer : ISynchronizer
    {
        private const string Ref = "../__json/schema";
        private readonly ILogger log;

        public int Order => -1000;

        public string Name => "Schemas";

        public SchemasSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public Task CleanupAsync(IFileSystem fs)
        {
            foreach (var file in GetSchemaFiles(fs))
            {
                file.Delete();
            }

            return Task.CompletedTask;
        }

        public async Task ExportAsync(ISyncService sync, SyncOptions options, ISession session)
        {
            var current = await session.Schemas.GetSchemasAsync(session.App);

            var schemaMap = current.Items.ToDictionary(x => x.Id, x => x.Name);

            foreach (var schema in current.Items.OrderBy(x => x.Name))
            {
                await log.DoSafeAsync($"Exporting '{schema.Name}'", async () =>
                {
                    var details = await session.Schemas.GetSchemaAsync(session.App, schema.Name);

                    var model = new SchemaModel
                    {
                        Name = schema.Name,
                        Schema = sync.Convert<SynchronizeSchemaDto>(details),
                        SchemaType = details.Type,
                        IsSingleton = details.IsSingleton
                    };

                    model.Schema.MapReferences(schemaMap);

                    await sync.WriteWithSchema(new FilePath($"schemas", $"{schema.Name}.json"), model, Ref);
                });
            }
        }

        public async Task ImportAsync(ISyncService sync, SyncOptions options, ISession session)
        {
            var createModels =
                GetSchemaFiles(sync.FileSystem)
                    .Select(x => sync.Read<SchemaCreateModel>(x, log))
                    .ToList();

            if (!createModels.HasDistinctNames(x => x.Name))
            {
                log.WriteLine("ERROR: Can only sync schemas when all target schemas have distinct names.");
                return;
            }

            var current = await session.Schemas.GetSchemasAsync(session.App);

            var schemasByName = current.Items.ToDictionary(x => x.Name);

            if (options.Delete)
            {
                foreach (var name in current.Items.Select(x => x.Name))
                {
                    if (createModels.All(x => x.Name != name))
                    {
                        await log.DoSafeAsync($"Schema {name} deleting", async () =>
                        {
                            await session.Schemas.DeleteSchemaAsync(session.App, name);
                        });
                    }
                }
            }

            foreach (var model in createModels)
            {
                if (schemasByName.ContainsKey(model.Name))
                {
                    continue;
                }

                await log.DoSafeAsync($"Schema {model.Name} creating", async () =>
                {
                    var created = await session.Schemas.PostSchemaAsync(session.App, model.ToRequest());

                    schemasByName[model.Name] = created;
                });
            }

            var schemaMap = schemasByName.ToDictionary(x => x.Key, x => x.Value.Id);

            var models =
                GetSchemaFiles(sync.FileSystem)
                    .Select(x => sync.Read<SchemaModel>(x, log))
                    .ToList();

            foreach (var model in models)
            {
                model.Schema.MapReferences(schemaMap);

                var version = schemasByName[model.Name].Version;

                if (!options.Delete)
                {
                    model.Schema.NoFieldRecreation = true;
                }

                if (!options.Recreate)
                {
                    model.Schema.NoFieldRecreation = true;
                }

                await log.DoVersionedAsync($"Schema {model.Name} updating", version, async () =>
                {
                    var result = await session.Schemas.PutSchemaSyncAsync(session.App, model.Name, model.Schema);

                    return result.Version;
                });
            }
        }

        private static IEnumerable<IFile> GetSchemaFiles(IFileSystem fs)
        {
            foreach (var file in fs.GetFiles(new FilePath("schemas"), ".json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }
        }

        public async Task GenerateSchemaAsync(ISyncService sync)
        {
            await sync.WriteJsonSchemaAsync<SchemaModel>(new FilePath("schema.json"));

            var sample = new SchemaModel
            {
                Name = "my-schema",
                Schema = new SynchronizeSchemaDto
                {
                    Properties = new SchemaPropertiesDto
                    {
                        Label = "My Schema"
                    },
                    Fields = new List<UpsertSchemaFieldDto>
                    {
                        new UpsertSchemaFieldDto
                        {
                            Name = "my-string",
                            Properties = new StringFieldPropertiesDto
                            {
                                IsRequired = true
                            },
                            Partitioning = "invariant"
                        }
                    },
                    IsPublished = true
                }
            };

            await sync.WriteWithSchema(new FilePath("schemas", "__schema.json"), sample, Ref);
        }
    }
}
