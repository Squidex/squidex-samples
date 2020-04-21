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

        public string Name => "App";

        public AppSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public async Task SynchronizeAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
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
        }

        private async Task SynchronizeContributorsAsync(AppSettings settings, ISession session)
        {
            foreach (var (email, value) in settings.Contributors)
            {
                await log.DoSafeAsync($"Contributor '{email}' creating", async () =>
                {
                    var request = new AssignContributorDto { ContributorId = email, Role = value.Role, Invite = true };

                    await session.Apps.PostContributorAsync(session.App, request);
                });
            }
        }

        private async Task SynchronizeClientsAsync(AppSettings settings, SyncOptions options, ISession session)
        {
            var current = await session.Apps.GetClientsAsync(session.App);

            if (!options.NoDeletion)
            {
                foreach (var client in current.Items)
                {
                    var generatedClientId = $"{session.App}:{client.Id}";

                    if (settings.Clients.ContainsKey(client.Id) || session.ClientId.Equals(generatedClientId))
                    {
                        continue;
                    }

                    await log.DoSafeAsync($"Client '{client.Id}' deleting", async () =>
                    {
                        await session.Apps.DeleteClientAsync(session.App, client.Id);
                    });
                }
            }

            foreach (var (clientId, value) in settings.Clients)
            {
                var existing = current.Items.FirstOrDefault(x => x.Id == clientId);

                if (existing != null)
                {
                    continue;
                }

                await log.DoSafeAsync($"Client '{clientId}' creating", async () =>
                {
                    var request = new CreateClientDto { Id = clientId };

                    current = await session.Apps.PostClientAsync(session.App, request);
                });
            }

            foreach (var (clientId, value) in settings.Clients)
            {
                var existing = current.Items.FirstOrDefault(x => x.Id == clientId);

                if (existing == null || value.JsonEquals(existing))
                {
                    continue;
                }

                await log.DoSafeAsync($"Client '{clientId}' updating", async () =>
                {
                    var request = new UpdateClientDto { Role = value.Role };

                    await session.Apps.PutClientAsync(session.App, clientId, request);
                });
            }
        }

        private async Task SynchronizeLanguagesAsync(AppSettings settings, SyncOptions options, ISession session)
        {
            var current = await session.Apps.GetLanguagesAsync(session.App);

            if (!options.NoDeletion)
            {
                foreach (var language in current.Items)
                {
                    if (settings.Languages.ContainsKey(language.Iso2Code))
                    {
                        continue;
                    }

                    await log.DoSafeAsync($"Language '{language.Iso2Code}' deleting", async () =>
                    {
                        await session.Apps.DeleteLanguageAsync(session.App, language.Iso2Code);
                    });
                }
            }

            foreach (var (isoCode, value) in settings.Languages)
            {
                var existing = current.Items.FirstOrDefault(x => x.Iso2Code == isoCode);

                if (existing != null)
                {
                    continue;
                }

                await log.DoSafeAsync($"Language '{isoCode}' creating", async () =>
                {
                    var request = new AddLanguageDto { Language = isoCode };

                    current = await session.Apps.PostLanguageAsync(session.App, request);
                });
            }

            foreach (var (isoCode, value) in settings.Languages)
            {
                var existing = current.Items.FirstOrDefault(x => x.Iso2Code == isoCode);

                if (existing == null || value.JsonEquals(existing))
                {
                    continue;
                }

                await log.DoSafeAsync($"Language '{isoCode}' updating", async () =>
                {
                    var request = value;

                    await session.Apps.PutLanguageAsync(session.App, isoCode, request);
                });
            }
        }

        private async Task SynchronizeRolesAsync(AppSettings settings, SyncOptions options, ISession session)
        {
            var current = await session.Apps.GetRolesAsync(session.App);

            if (!options.NoDeletion)
            {
                foreach (var role in current.Items)
                {
                    if (settings.Roles.ContainsKey(role.Name) ||
                        role.IsDefaultRole ||
                        role.NumClients > 0 ||
                        role.NumContributors > 0)
                    {
                        continue;
                    }

                    await log.DoSafeAsync($"Role '{role.Name}' deleting", async () =>
                    {
                        await session.Apps.DeleteRoleAsync(session.App, role.Name);
                    });
                }
            }

            foreach (var (roleName, value) in settings.Roles)
            {
                var existing = current.Items.FirstOrDefault(x => x.Name == roleName);

                if (existing != null)
                {
                    continue;
                }

                await log.DoSafeAsync($"Role '{roleName}' creating", async () =>
                {
                    var request = new AddRoleDto { Name = roleName };

                    current = await session.Apps.PostRoleAsync(session.App, request);
                });
            }

            foreach (var (roleName, value) in settings.Roles)
            {
                var existing = current.Items.FirstOrDefault(x => x.Name == roleName);

                if (existing == null || value.JsonEquals(existing))
                {
                    continue;
                }

                await log.DoSafeAsync($"Role '{roleName}' updating", async () =>
                {
                    var request = new UpdateRoleDto { Permissions = value.Permissions };

                    await session.Apps.PutRoleAsync(session.App, roleName, request);
                });
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
