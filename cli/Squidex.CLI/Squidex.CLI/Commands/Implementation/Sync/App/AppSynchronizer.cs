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

        public Task CleanupAsync(DirectoryInfo directoryInfo)
        {
            return Task.CompletedTask;
        }

        public async Task ExportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var model = new AppModel
            {
                Contributors = new Dictionary<string, AppContributorModel>()
            };

            await log.DoSafeAsync("Exporting clients", async () =>
            {
                var clients = await session.Apps.GetClientsAsync(session.App);

                model.Clients = new Dictionary<string, AppClientModel>();

                foreach (var client in clients.Items)
                {
                    model.Clients[client.Name] = new AppClientModel
                    {
                        Name = client.Name,
                        Role = client.Role
                    };
                }
            });

            await log.DoSafeAsync("Exporting languages", async () =>
            {
                var languages = await session.Apps.GetLanguagesAsync(session.App);

                model.Languages = new Dictionary<string, UpdateLanguageDto>();

                foreach (var language in languages.Items)
                {
                    model.Languages[language.Iso2Code] = new UpdateLanguageDto
                    {
                        Fallback = language.Fallback,
                        IsMaster = language.IsMaster,
                        IsOptional = language.IsOptional
                    };
                }
            });

            await log.DoSafeAsync("Exporting Roles", async () =>
            {
                var roles = await session.Apps.GetRolesAsync(session.App);

                model.Roles = new Dictionary<string, AppRoleModel>();

                foreach (var role in roles.Items)
                {
                    model.Roles[role.Name] = new AppRoleModel
                    {
                        Permissions = role.Permissions.ToArray()
                    };
                }
            });

            await jsonHelper.WriteWithSchema(directoryInfo, "app.json", model, "__json/app");
        }

        public async Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var appFile = new FileInfo(Path.Combine(directoryInfo.FullName, "app.json"));

            if (!appFile.Exists)
            {
                log.WriteLine("App settings synchronization skipped: {0} does not exist", appFile.FullName);
                return;
            }

            var model = jsonHelper.Read<AppModel>(appFile, log);

            await SynchronizeClientsAsync(model, options, session);
            await SynchronizeRolesAsync(model, options, session);
            await SynchronizeContributorsAsync(model, session);
            await SynchronizeLanguagesAsync(model, options, session);
        }

        private async Task SynchronizeContributorsAsync(AppModel model, ISession session)
        {
            foreach (var (email, value) in model.Contributors)
            {
                await log.DoSafeAsync($"Contributor '{email}' creating", async () =>
                {
                    var request = new AssignContributorDto { ContributorId = email, Role = value.Role, Invite = true };

                    await session.Apps.PostContributorAsync(session.App, request);
                });
            }
        }

        private async Task SynchronizeClientsAsync(AppModel model, SyncOptions options, ISession session)
        {
            var current = await session.Apps.GetClientsAsync(session.App);

            if (!options.NoDeletion)
            {
                foreach (var client in current.Items)
                {
                    var generatedClientId = $"{session.App}:{client.Id}";

                    if (model.Clients.ContainsKey(client.Id) || session.ClientId.Equals(generatedClientId))
                    {
                        continue;
                    }

                    await log.DoSafeAsync($"Client '{client.Id}' deleting", async () =>
                    {
                        await session.Apps.DeleteClientAsync(session.App, client.Id);
                    });
                }
            }

            foreach (var (clientId,  _) in model.Clients)
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

            foreach (var (clientId, value) in model.Clients)
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

        private async Task SynchronizeLanguagesAsync(AppModel model, SyncOptions options, ISession session)
        {
            var current = await session.Apps.GetLanguagesAsync(session.App);

            if (!options.NoDeletion)
            {
                foreach (var language in current.Items)
                {
                    if (model.Languages.ContainsKey(language.Iso2Code))
                    {
                        continue;
                    }

                    await log.DoSafeAsync($"Language '{language.Iso2Code}' deleting", async () =>
                    {
                        await session.Apps.DeleteLanguageAsync(session.App, language.Iso2Code);
                    });
                }
            }

            foreach (var (isoCode, _) in model.Languages)
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

            foreach (var (isoCode, value) in model.Languages)
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

        private async Task SynchronizeRolesAsync(AppModel model, SyncOptions options, ISession session)
        {
            var current = await session.Apps.GetRolesAsync(session.App);

            if (!options.NoDeletion)
            {
                foreach (var role in current.Items)
                {
                    if (model.Roles.ContainsKey(role.Name) ||
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

            foreach (var (roleName, _) in model.Roles)
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

            foreach (var (roleName, value) in model.Roles)
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
            await jsonHelper.WriteJsonSchemaAsync<AppModel>(directoryInfo, "app.json");

            var sample = new AppModel
            {
                Roles = new Dictionary<string, AppRoleModel>
                {
                    ["custom"] = new AppRoleModel
                    {
                        Permissions = new[]
                        {
                            "schemas.*"
                        }
                    }
                },
                Clients = new Dictionary<string, AppClientModel>
                {
                    ["test"] = new AppClientModel
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
                Contributors = new Dictionary<string, AppContributorModel>
                {
                    ["sebastian@squidex.io"] = new AppContributorModel
                    {
                        Role = "Owner"
                    }
                }
            };

            await jsonHelper.WriteWithSchema(directoryInfo, "__app.json", sample, "__json/app");
        }
    }
}
