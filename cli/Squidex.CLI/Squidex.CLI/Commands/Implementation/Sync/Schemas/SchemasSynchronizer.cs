// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Schemas
{
    public sealed class SchemasSynchronizer : ISynchronizer
    {
        private readonly ILogger log;

        public int Order => -1000;

        public string Name => "Schemas";

        public SchemasSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public Task CleanupAsync(DirectoryInfo directoryInfo)
        {
            foreach (var file in GetSchemaFiles(directoryInfo))
            {
                file.Delete();
            }

            return Task.CompletedTask;
        }

        public async Task ExportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var current = await session.Schemas.GetSchemasAsync(session.App);

            var schemaMap = current.Items.ToDictionary(x => x.Id, x => x.Name);

            foreach (var schema in current.Items.OrderBy(x => x.Name))
            {
                await log.DoSafeAsync($"Exporting '{schema.Name}'", async () =>
                {
                    var details = await session.Schemas.GetSchemaAsync(session.App, schema.Name);

                    var model = new SchemeModel
                    {
                        Name = schema.Name,
                        IsSingleton = details.IsSingleton
                    };

                    model.Schema = jsonHelper.Convert<SynchronizeSchemaDto>(details);

                    MapReferences(model.Schema, schemaMap);

                    await jsonHelper.WriteWithSchema(directoryInfo, $"schemas/{schema.Name}.json", model, "../__json/schema");
                });
            }
        }

        public async Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var createModels =
                GetSchemaFiles(directoryInfo)
                    .Select(x => jsonHelper.Read<SchemaCreateModel>(x, log))
                    .ToList();

            if (!createModels.HasDistinctNames(x => x.Name))
            {
                log.WriteLine("ERROR: Can only sync schemas when all target schemas have distinct names.");
                return;
            }

            var current = await session.Schemas.GetSchemasAsync(session.App);

            var schemasByName = current.Items.ToDictionary(x => x.Name);

            if (!options.NoDeletion)
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

            foreach (var schema in createModels)
            {
                if (schemasByName.ContainsKey(schema.Name))
                {
                    continue;
                }

                await log.DoSafeAsync($"Schema {schema.Name} creating", async () =>
                {
                    var request = new CreateSchemaDto
                    {
                        Name = schema.Name,
                        IsSingleton = schema.IsSingleton
                    };

                    var created = await session.Schemas.PostSchemaAsync(session.App, request);

                    schemasByName[schema.Name] = created;
                });
            }

            var schemaMap = schemasByName.ToDictionary(x => x.Key, x => x.Value.Id);

            var models =
                GetSchemaFiles(directoryInfo)
                    .Select(x => jsonHelper.Read<SchemeModel>(x, log))
                    .ToList();

            foreach (var schema in models)
            {
                MapReferences(schema.Schema, schemaMap);

                var version = schemasByName[schema.Name].Version;

                if (options.NoDeletion)
                {
                    schema.Schema.NoFieldDeletion = true;
                    schema.Schema.NoFieldRecreation = true;
                }

                await log.DoVersionedAsync($"Schema {schema.Name} updating", version, async () =>
                {
                    var result = await session.Schemas.PutSchemaSyncAsync(session.App, schema.Name, schema.Schema);

                    return result.Version;
                });
            }
        }

        private static IEnumerable<FileInfo> GetSchemaFiles(DirectoryInfo directoryInfo)
        {
            foreach (var file in directoryInfo.GetFiles("schemas/*.json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }
        }

        public async Task GenerateSchemaAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<SchemeModel>(directoryInfo, "schema.json");

            var sample = new SchemeModel
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

            await jsonHelper.WriteWithSchema(directoryInfo, "schemas/__schema.json", sample, "../__json/schema");
        }

        private static void MapReferences(SynchronizeSchemaDto schema, Dictionary<string, string> map)
        {
            if (schema.Fields != null)
            {
                foreach (var field in schema.Fields)
                {
                    MapReferences(field, map);
                }
            }
        }

        private static void MapReferences(UpsertSchemaFieldDto field, Dictionary<string, string> map)
        {
            MapReferences(field.Properties, map);

            if (field.Nested != null)
            {
                foreach (var nested in field.Nested)
                {
                    MapReferences(nested.Properties, map);
                }
            }
        }

        private static void MapReferences(FieldPropertiesDto properties, Dictionary<string, string> map)
        {
            if (properties is ReferencesFieldPropertiesDto references)
            {
                if (references.SchemaIds != null && references.SchemaIds.Any())
                {
                    var names = new List<string>();

                    foreach (var id in references.SchemaIds)
                    {
                        if (map.TryGetValue(id, out var target))
                        {
                            names.Add(target);
                        }
                    }

                    references.SchemaIds = names;
                }
            }
        }
    }
}