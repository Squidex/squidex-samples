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
using Squidex.CLI.Commands.Implementation.Sync.Model;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public sealed class Synchronizer
    {
        private readonly DirectoryInfo directoryInfo;
        private readonly Dictionary<string, Guid> schemaNames = new Dictionary<string, Guid>();
        private readonly ILogger log;
        private readonly ISession session;
        private readonly SyncOptions options;

        public Synchronizer(ILogger log, string path, ISession session, SyncOptions options)
        {
            directoryInfo = Directory.CreateDirectory(path);

            this.session = session;
            this.options = options;

            this.log = log;
        }

        public async Task SyncAsync()
        {
            await SynchronizeSchemas();
        }

        private async Task SynchronizeSchemas()
        {
            log.WriteLine();
            log.WriteLine("Schemas synchronizing");

            var existingSchemas = await session.Schemas.GetSchemasAsync(session.App);

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
                var settings = JsonHelper.Read<SchemaSettingsNameOnly>(file);

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
                    var json = JsonHelper.Read<SchemaSettings>(file);

                    await session.Schemas.PutSchemaSyncAsync(session.App, json.Name, json.Schema);
                });
            }
        }

        private IEnumerable<FileInfo> GetSchemaFiles()
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
}
