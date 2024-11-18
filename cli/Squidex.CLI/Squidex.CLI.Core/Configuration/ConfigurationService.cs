// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Configuration;

public sealed class ConfigurationService(IConfigurationStore configurationStore) : IConfigurationService
{
    private const string CloudUrl = "https://cloud.squidex.io";
    private readonly ConfigurationModel configuration = configurationStore.Get<ConfigurationModel>(".configuration").Value ?? new ConfigurationModel();

    private sealed class ConfigurationModel
    {
        public Dictionary<string, ConfiguredApp> Apps { get; } = [];

        public string? CurrentApp { get; set; }
    }

    private void Save()
    {
        try
        {
            configurationStore.Set(".configuration", configuration);
        }
        catch (Exception ex)
        {
            throw new CLIException("Failed to save configuration file.", ex);
        }
    }

    public void Reset()
    {
        configuration.Apps.Clear();
        configuration.CurrentApp = null;

        Save();
    }

    public void Upsert(string config, ConfiguredApp appConfig)
    {
        if (string.IsNullOrWhiteSpace(appConfig.ServiceUrl))
        {
            appConfig = appConfig with { ServiceUrl = CloudUrl };
        }

        configuration.Apps[config] = appConfig;

        if (string.IsNullOrWhiteSpace(configuration.CurrentApp))
        {
            configuration.CurrentApp = config;
        }

        Save();
    }

    public void UseApp(string entry)
    {
        if (!configuration.Apps.ContainsKey(entry))
        {
            throw new CLIException("App config with the name does not exist.");
        }

        configuration.CurrentApp = entry;

        Save();
    }

    public void Remove(string entry)
    {
        if (!configuration.Apps.ContainsKey(entry))
        {
            throw new CLIException("App config with the name does not exist.");
        }

        configuration.Apps.Remove(entry);

        if (configuration.CurrentApp == entry)
        {
            configuration.CurrentApp = configuration.Apps.FirstOrDefault().Key;
        }

        Save();
    }

    public ISession StartSession(string appName, bool emulate = false)
    {
        if (!string.IsNullOrWhiteSpace(appName) && configuration.Apps.TryGetValue(appName, out var app))
        {
            var options = CreateOptions(app, emulate);

            return new Session(configurationStore.WorkingDirectory, new SquidexClient(options));
        }

        if (!string.IsNullOrWhiteSpace(configuration.CurrentApp) && configuration.Apps.TryGetValue(configuration.CurrentApp, out app))
        {
            var options = CreateOptions(app, emulate);

            return new Session(configurationStore.WorkingDirectory, new SquidexClient(options));
        }

        throw new CLIException("Cannot find valid configuration.");
    }

    private static SquidexOptions CreateOptions(ConfiguredApp app, bool emulate)
    {
        var options = new SquidexOptions
        {
            Url = app.ServiceUrl,
            AppName = app.Name,
            ClientId = app.ClientId,
            ClientSecret = app.ClientSecret,
            Timeout = TimeSpan.FromHours(1)
        };

        options.UseFallbackSerializer();
        options.IgnoreSelfSignedCertificates = app.IgnoreSelfSigned;

        if (emulate)
        {
            options.ClientProvider = new GetOnlyHttpClientProvider(options);
        }

        return options;
    }

    public (string? CurrentApp, (string Name, ConfiguredApp)[] Apps) GetConfiguration()
    {
        return (configuration.CurrentApp, configuration.Apps.Select(x => (x.Key, x.Value)).ToArray());
    }
}
