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
            await SynchronizeApp();
            await SynchronizeSchemas();
        }

        private async Task SynchronizeApp()
        {
            log.WriteLine();
            log.WriteLine("App settings synchronizing");

            var appFile = new FileInfo(Path.Combine(directoryInfo.FullName, "app.json"));

            if (!appFile.Exists)
            {
                log.WriteLine("App settings synchronization skipped: {0} does not exist", appFile.FullName);
                return;
            }

            var settings = JsonHelper.Read<AppSettings>(appFile, log);

            await SynchronizeClientsAsync(settings);
            await SynchronizeContributorsAsync(settings);
            await SynchronizeLanguagesAsync(settings);
            await SynchronizeRolesAsync(settings);

            log.WriteLine("App settings synchronized");
        }

        private async Task SynchronizeContributorsAsync(AppSettings settings)
        {
            foreach (var (email, role) in settings.Contributors)
            {
                await log.DoSafeAsync($"Contributor '{email}' creating", async () =>
                {
                    var request = new AssignContributorDto
                    {
                        ContributorId = email, Role = role, Invite = true
                    };

                    await session.Apps.PostContributorAsync(session.App, request);
                });
            }
        }

        private async Task SynchronizeClientsAsync(AppSettings settings)
        {
            var existingClients = await session.Apps.GetClientsAsync(session.App);

            if (!options.NoDeletion)
            {
                foreach (var client in existingClients.Items)
                {
                    var generatedClientId = $"{session.App}:{client.Id}";

                    if (!settings.Clients.ContainsKey(client.Id) && !session.ClientId.Equals(generatedClientId))
                    {
                        await log.DoSafeAsync($"Client '{client.Id}' deleting", async () =>
                        {
                            await session.Apps.DeleteClientAsync(session.App, client.Id);
                        });
                    }
                }
            }

            foreach (var (clientId, value) in settings.Clients)
            {
                var existing = existingClients.Items.FirstOrDefault(x => x.Id == clientId);

                if (existing == null)
                {
                    await log.DoSafeAsync($"Client '{clientId}' creating", async () =>
                    {
                        var request = new CreateClientDto
                        {
                            Id = clientId
                        };

                        existingClients = await session.Apps.PostClientAsync(session.App, request);
                    });
                }
            }

            foreach (var (clientId, value) in settings.Clients)
            {
                var existing = existingClients.Items.FirstOrDefault(x => x.Id == clientId);

                if (existing != null && !value.JsonEquals(existing))
                {
                    await log.DoSafeAsync($"Client '{clientId}' updating", async () =>
                    {
                        var request = new UpdateClientDto
                        {
                            Role = value.Role
                        };

                        await session.Apps.PutClientAsync(session.App, clientId, request);
                    });
                }
            }
        }

        private async Task SynchronizeLanguagesAsync(AppSettings settings)
        {
            var existingLanguages = await session.Apps.GetLanguagesAsync(session.App);

            if (!options.NoDeletion)
            {
                foreach (var language in existingLanguages.Items)
                {
                    if (!settings.Languages.ContainsKey(language.Iso2Code))
                    {
                        await log.DoSafeAsync($"Language '{language.Iso2Code}' deleting", async () =>
                        {
                            await session.Apps.DeleteLanguageAsync(session.App, language.Iso2Code);
                        });
                    }
                }
            }

            foreach (var (isoCode, value) in settings.Languages)
            {
                var existing = existingLanguages.Items.FirstOrDefault(x => x.Iso2Code == isoCode);

                if (existing == null)
                {
                    await log.DoSafeAsync($"Language '{isoCode}' creating", async () =>
                    {
                        var request = new AddLanguageDto
                        {
                            Language = isoCode
                        };

                        existingLanguages = await session.Apps.PostLanguageAsync(session.App, request);
                    });
                }
            }

            foreach (var (isoCode, value) in settings.Languages)
            {
                var existing = existingLanguages.Items.FirstOrDefault(x => x.Iso2Code == isoCode);

                if (existing != null && !value.JsonEquals(existing))
                {
                    await log.DoSafeAsync($"Language '{isoCode}' updating", async () =>
                    {
                        var request = value;

                        await session.Apps.PutLanguageAsync(session.App, isoCode, request);
                    });
                }
            }
        }

        private async Task SynchronizeRolesAsync(AppSettings settings)
        {
            var existingRoles = await session.Apps.GetRolesAsync(session.App);

            if (!options.NoDeletion)
            {
                foreach (var role in existingRoles.Items)
                {
                    if (!settings.Roles.ContainsKey(role.Name) && !role.IsDefaultRole && role.NumClients == 0 && role.NumContributors == 0)
                    {
                        await log.DoSafeAsync($"Role '{role.Name}' deleting", async () =>
                        {
                            await session.Apps.DeleteRoleAsync(session.App, role.Name);
                        });
                    }
                }
            }

            foreach (var (roleName, value) in settings.Roles)
            {
                var existing = existingRoles.Items.FirstOrDefault(x => x.Name == roleName);

                if (existing == null)
                {
                    await log.DoSafeAsync($"Role '{roleName}' creating", async () =>
                    {
                        var request = new AddRoleDto
                        {
                            Name = roleName
                        };

                        existingRoles = await session.Apps.PostRoleAsync(session.App, request);
                    });
                }
            }

            foreach (var (roleName, value) in settings.Roles)
            {
                var existing = existingRoles.Items.FirstOrDefault(x => x.Name == roleName);

                if (existing != null && !value.JsonEquals(existing))
                {
                    await log.DoSafeAsync($"Role '{roleName}' updating", async () =>
                    {
                        var request = new UpdateRoleDto
                        {
                            Permissions = value.Permissions
                        };

                        await session.Apps.PutRoleAsync(session.App, roleName, request);
                    });
                }
            }
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
                var settings = JsonHelper.Read<SchemaSettingsNameOnly>(file, log);

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
                    var json = JsonHelper.Read<SchemaSettings>(file, log);

                    await session.Schemas.PutSchemaSyncAsync(session.App, json.Name, json.Schema);
                });
            }

            log.WriteLine("Schemas synchronized");
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
