// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Schemas
{
    public sealed class SchemasSynchronizer : ISynchronizer
    {
        private readonly ILogger log;

        public int Order => -1000;

        public SchemasSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public async Task SynchronizeAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            log.WriteLine();
            log.WriteLine("Schemas synchronizing");

            var existingSchemas = await session.Schemas.GetSchemasAsync(session.App);

            var schemaNames = new Dictionary<string, Guid>();
            var schemasToAdd = new HashSet<string>();
            var schemasToDelete = new HashSet<string>();

            var existingSchemaNames = new HashSet<string>();

            foreach (var schema in existingSchemas.Items)
            {
                existingSchemaNames.Add(schema.Name);

                schemaNames[schema.Name] = schema.Id;
            }

            foreach (var file in GetSchemaFiles())
            {
                var settings = jsonHelper.Read<SchemaSettingsNameOnly>(file, log);

                if (!string.IsNullOrWhiteSpace(settings.Name))
                {
                    schemaNames[settings.Name] = Guid.Empty;

                    if (!existingSchemaNames.Contains(settings.Name))
                    {
                        schemasToAdd.Add(settings.Name);
                    }
                }
            }

            if (!options.NoDeletion)
            {
                foreach (var existingName in existingSchemaNames)
                {
                    if (!schemaNames.ContainsKey(existingName))
                    {
                        schemasToDelete.Add(existingName);
                    }
                }
            }

            foreach (var schemaToAdd in schemasToAdd)
            {
                await log.DoSafeAsync($"Creating schema {schemaToAdd}", async () =>
                {
                    var request = new CreateSchemaDto
                    {
                        Name = schemaToAdd
                    };

                    var created = await session.Schemas.PostSchemaAsync(session.App, request);

                    schemaNames[schemaToAdd] = created.Id;
                });
            }

            foreach (var schemaToDelete in schemasToDelete)
            {
                await log.DoSafeAsync($"Delete schema {schemaToDelete}", async () =>
                {
                    await session.Schemas.DeleteSchemaAsync(session.App, schemaToDelete);
                });
            }

            foreach (var file in GetSchemaFiles())
            {
                await log.DoSafeAsync($"Synchronizing schema file {file.Name}", async () =>
                {
                    var json = jsonHelper.Read<SchemaSettings>(file, log);

                    await session.Schemas.PutSchemaSyncAsync(session.App, json.Name, json.Schema);
                });
            }

            log.WriteLine("Schemas synchronized");

            IEnumerable<FileInfo> GetSchemaFiles()
            {
                foreach (var file in directoryInfo.GetFiles("schemas\\*.json"))
                {
                    if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return file;
                    }
                }
            }
        }

        public async Task GenerateSchemaAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<SchemaSettings>(directoryInfo, "schema.json");

            var sample = new SchemaSettings
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

            await jsonHelper.WriteSampleAsync(directoryInfo, "schemas/__schema.json", sample, "../__json/schema");
        }
    }
}
