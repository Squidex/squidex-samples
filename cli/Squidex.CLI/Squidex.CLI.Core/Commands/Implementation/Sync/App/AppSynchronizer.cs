// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.App;

public sealed class AppSynchronizer : ISynchronizer
{
    private const string Ref = "__json/app";
    private readonly ILogger log;

    public string Name => "App";

    public string Description => "Synchronize all app settings: clients, contributors, roles, languages and asset scripts. But not: workflows.";

    public AppSynchronizer(ILogger log)
    {
        this.log = log;
    }

    public Task CleanupAsync(IFileSystem fs)
    {
        return Task.CompletedTask;
    }

    public async Task ExportAsync(ISyncService sync, SyncOptions options, ISession session)
    {
        var model = new AppModel
        {
            Contributors = new Dictionary<string, AppContributorModel>()
        };

        await log.DoSafeAsync("Exporting clients", async () =>
        {
            var clients = await session.Client.Apps.GetClientsAsync();

            model.Clients = new Dictionary<string, AppClientModel>();

            foreach (var client in clients.Items)
            {
                model.Clients[client.Name] = client.ToModel();
            }
        });

        await log.DoSafeAsync("Exporting languages", async () =>
        {
            var languages = await session.Client.Apps.GetLanguagesAsync();

            model.Languages = new Dictionary<string, UpdateLanguageDto>();

            foreach (var language in languages.Items)
            {
                model.Languages[language.Iso2Code] = language.ToModel();
            }
        });

        await log.DoSafeAsync("Exporting Roles", async () =>
        {
            var roles = await session.Client.Apps.GetRolesAsync();

            model.Roles = new Dictionary<string, AppRoleModel>();

            foreach (var role in roles.Items.Where(x => !x.IsDefaultRole))
            {
                model.Roles[role.Name] = role.ToModel();
            }
        });

        await log.DoSafeAsync("Exporting asset scripts", async () =>
        {
            var assetScripts = await session.Client.Apps.GetAssetScriptsAsync();

            model.AssetScripts = assetScripts.ToModel();
        });

        await sync.WriteWithSchema(new FilePath("app.json"), model, Ref);
    }

    public Task DescribeAsync(ISyncService sync, MarkdownWriter writer)
    {
        var appFile = sync.FileSystem.GetFile(new FilePath("app.json"));

        if (!appFile.Exists)
        {
            return Task.CompletedTask;
        }

        var model = sync.Read<AppModel>(appFile, log);

        if (model.Clients.Count > 0)
        {
            var rows = model.Clients.Select(x => new object?[] { x.Key, x.Value.Name, x.Value.Role }).OrderBy(x => x[0]).ToArray();

            writer.H3("Clients");
            writer.Paragraph($"{rows.Length} client(s).");
            writer.Table(new[] { "Name", "Label", "Role" }, rows);
        }

        if (model.Roles.Count > 0)
        {
            var rows = model.Roles.Select(x => new object[] { x.Key, x.Value.Permissions.Count }).OrderBy(x => x[0]).ToArray();

            writer.H3("Roles");
            writer.Paragraph($"{rows.Length} role(s).");
            writer.Table(new[] { "Name", "Permissions" }, rows);
        }

        if (model.Contributors.Count > 0)
        {
            var rows = model.Contributors.Select(x => new object[] { x.Key, x.Value.Role }).OrderBy(x => x[0]).ToArray();

            writer.H3("Contributors");
            writer.Paragraph($"{rows.Length} contributor(s).");
            writer.Table(new[] { "Id", "Role" }, rows);
        }

        if (model.Languages.Count > 0)
        {
            var rows = model.Languages.Select(x => new object[] { x.Key, x.Value.IsMaster == true ? "y" : "n" }).OrderBy(x => x[0]).ToArray();

            writer.H3("Languages");
            writer.Paragraph($"{rows.Length} language(s).");
            writer.Table(new[] { "Code", "Master" }, rows);
        }

        return Task.CompletedTask;
    }

    public async Task ImportAsync(ISyncService sync, SyncOptions options, ISession session)
    {
        var appFile = sync.FileSystem.GetFile(new FilePath("app.json"));

        if (!appFile.Exists)
        {
            log.WriteLine("App settings synchronization skipped: {0} does not exist", appFile.FullName);
            return;
        }

        var model = sync.Read<AppModel>(appFile, log);

        await SynchronizeClientsAsync(model, options, session);
        await SynchronizeRolesAsync(model, options, session);
        await SynchronizeContributorsAsync(model, session);
        await SynchronizeLanguagesAsync(model, options, session);
        await SynchronizeAssetScriptsAsync(model, session);
    }

    private async Task SynchronizeContributorsAsync(AppModel model, ISession session)
    {
        foreach (var (email, value) in model.Contributors)
        {
            await log.DoSafeAsync($"Contributor '{email}' creating", async () =>
            {
                var request = new AssignContributorDto { ContributorId = email, Role = value.Role, Invite = true };

                await session.Client.Apps.PostContributorAsync(request);
            });
        }
    }

    private async Task SynchronizeClientsAsync(AppModel model, SyncOptions options, ISession session)
    {
        var current = await session.Client.Apps.GetClientsAsync();

        if (options.Delete)
        {
            foreach (var client in current.Items)
            {
                var generatedClientId = $"{session.App}:{client.Id}";

                if (model.Clients.ContainsKey(client.Id) || session.ClientId.Equals(generatedClientId, StringComparison.Ordinal))
                {
                    continue;
                }

                await log.DoSafeAsync($"Client '{client.Id}' deleting", async () =>
                {
                    await session.Client.Apps.DeleteClientAsync(client.Id);
                });
            }
        }

        foreach (var (clientId, _) in model.Clients)
        {
            var existing = current.Items.Find(x => x.Id == clientId);

            if (existing != null)
            {
                continue;
            }

            await log.DoSafeAsync($"Client '{clientId}' creating", async () =>
            {
                var request = new CreateClientDto { Id = clientId };

                current = await session.Client.Apps.PostClientAsync(request);
            });
        }

        foreach (var (clientId, client) in model.Clients)
        {
            var existing = current.Items.Find(x => x.Id == clientId);

            if (existing == null || client.JsonEquals(existing))
            {
                continue;
            }

            if (!options.UpdateCurrentClient && session.ClientId.Equals(clientId, StringComparison.Ordinal))
            {
                continue;
            }

            await log.DoSafeAsync($"Client '{clientId}' updating", async () =>
            {
                var request = client.ToUpdate();

                await session.Client.Apps.PutClientAsync(clientId, request);
            });
        }
    }

    private async Task SynchronizeLanguagesAsync(AppModel model, SyncOptions options, ISession session)
    {
        var current = await session.Client.Apps.GetLanguagesAsync();

        if (options.Delete)
        {
            foreach (var language in current.Items)
            {
                if (model.Languages.ContainsKey(language.Iso2Code))
                {
                    continue;
                }

                await log.DoSafeAsync($"Language '{language.Iso2Code}' deleting", async () =>
                {
                    await session.Client.Apps.DeleteLanguageAsync(language.Iso2Code);
                });
            }
        }

        foreach (var (isoCode, _) in model.Languages)
        {
            var existing = current.Items.Find(x => x.Iso2Code == isoCode);

            if (existing != null)
            {
                continue;
            }

            await log.DoSafeAsync($"Language '{isoCode}' creating", async () =>
            {
                var request = new AddLanguageDto { Language = isoCode };

                current = await session.Client.Apps.PostLanguageAsync(request);
            });
        }

        foreach (var (isoCode, language) in model.Languages)
        {
            var existing = current.Items.Find(x => x.Iso2Code == isoCode);

            if (existing == null || language.JsonEquals(existing))
            {
                continue;
            }

            await log.DoSafeAsync($"Language '{isoCode}' updating", async () =>
            {
                await session.Client.Apps.PutLanguageAsync(isoCode, language);
            });
        }
    }

    private async Task SynchronizeRolesAsync(AppModel model, SyncOptions options, ISession session)
    {
        var current = await session.Client.Apps.GetRolesAsync();

        if (options.Delete)
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
                    await session.Client.Apps.DeleteRoleAsync(role.Name);
                });
            }
        }

        foreach (var (roleName, _) in model.Roles)
        {
            var existing = current.Items.Find(x => x.Name == roleName);

            if (existing != null)
            {
                continue;
            }

            await log.DoSafeAsync($"Role '{roleName}' creating", async () =>
            {
                var request = new AddRoleDto { Name = roleName };

                current = await session.Client.Apps.PostRoleAsync(request);
            });
        }

        foreach (var (roleName, role) in model.Roles)
        {
            var existing = current.Items.Find(x => x.Name == roleName);

            if (existing == null || existing.IsDefaultRole)
            {
                continue;
            }

            await log.DoSafeAsync($"Role '{roleName}' updating", async () =>
            {
                var request = role.ToUpdate();

                await session.Client.Apps.PutRoleAsync(roleName, request);
            });
        }
    }

    private async Task SynchronizeAssetScriptsAsync(AppModel model, ISession session)
    {
        if (model.AssetScripts == null)
        {
            return;
        }

        await log.DoSafeAsync("Asset scripts updating", async () =>
        {
            var request = model.AssetScripts?.ToUpdate() ?? new UpdateAssetScriptsDto();

            await session.Client.Apps.PutAssetScriptsAsync(request);
        });
    }

    public async Task GenerateSchemaAsync(ISyncService sync)
    {
        await sync.WriteJsonSchemaAsync<AppModel>(new FilePath("app.json"));

        var sample = new AppModel
        {
            Roles = new Dictionary<string, AppRoleModel>
            {
                ["custom"] = new AppRoleModel
                {
                    Permissions = new List<string>
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
            },
            AssetScripts = new AssetScriptsModel()
        };

        await sync.WriteWithSchema(new FilePath("__app.json"), sample, Ref);
    }
}
