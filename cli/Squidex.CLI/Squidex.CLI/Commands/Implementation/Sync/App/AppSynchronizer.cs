// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.App
{
    public sealed class AppSynchronizer : ISynchronizer
    {
        private readonly ILogger log;

        public AppSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public async Task SynchronizeAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            log.WriteLine();
            log.WriteLine("App settings synchronizing");

            var appFile = new FileInfo(Path.Combine(directoryInfo.FullName, "app.json"));

            if (!appFile.Exists)
            {
                log.WriteLine("App settings synchronization skipped: {0} does not exist", appFile.FullName);
                return;
            }

            var settings = jsonHelper.Read<AppSettings>(appFile, log);

            await SynchronizeClientsAsync(settings, options, session);
            await SynchronizeContributorsAsync(settings, session);
            await SynchronizeLanguagesAsync(settings, options, session);
            await SynchronizeRolesAsync(settings, options, session);

            log.WriteLine("App settings synchronized");
        }

        private async Task SynchronizeContributorsAsync(AppSettings settings, ISession session)
        {
            foreach (var (email, value) in settings.Contributors)
            {
                await log.DoSafeAsync($"Contributor '{email}' creating", async () =>
                {
                    var request = new AssignContributorDto
                    {
                        ContributorId = email,
                        Role = value.Role,
                        Invite = true
                    };

                    await session.Apps.PostContributorAsync(session.App, request);
                });
            }
        }

        private async Task SynchronizeClientsAsync(AppSettings settings, SyncOptions options, ISession session)
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

        private async Task SynchronizeLanguagesAsync(AppSettings settings, SyncOptions options, ISession session)
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

        private async Task SynchronizeRolesAsync(AppSettings settings, SyncOptions options, ISession session)
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

        public async Task GenerateSchemaAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<AppSettings>(directoryInfo, "app.json");

            var sample = new AppSettings
            {
                Roles = new Dictionary<string, AppRoleSetting>
                {
                    ["custom"] = new AppRoleSetting
                    {
                        Permissions = new string[]
                        {
                            "schemas.*"
                        }
                    }
                },
                Clients = new Dictionary<string, AppClientSetting>
                {
                    ["test"] = new AppClientSetting
                    {
                        Role = "Owner"
                    }
                },
                Languages = new Dictionary<string, UpdateLanguageDto>
                {
                    ["en"] = new UpdateLanguageDto
                    {
                        IsMaster = true
                    }
                },
                Contributors = new Dictionary<string, AppContributorSetting>
                {
                    ["sebastian@squidex.io"] = new AppContributorSetting
                    {
                        Role = "Owner"
                    }
                }
            };

            await jsonHelper.WriteSampleAsync(directoryInfo, "__app.json", sample, "__json/app");
        }
    }
}
