// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Configuration
{
    public sealed class ConfigurationService : IConfigurationService
    {
        private const string CloudUrl = "https://cloud.squidex.io";
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();
        private readonly Configuration configuration;
        private string sessionApp;

        public ConfigurationService()
        {
            configuration = LoadConfiguration();
        }

        private Configuration LoadConfiguration()
        {
            try
            {
                var file = new FileInfo(".configuration");

                using (var stream = file.OpenRead())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        using (var jsonReader = new JsonTextReader(streamReader))
                        {
                            var result = jsonSerializer.Deserialize<Configuration>(jsonReader);

                            foreach (var (key, config) in result.Apps)
                            {
                                if (string.IsNullOrWhiteSpace(config.Name))
                                {
                                    config.Name = key;
                                }
                            }

                            return result;
                        }
                    }
                }
            }
            catch
            {
                return new Configuration();
            }
        }

        private void Save()
        {
            try
            {
                var file = new FileInfo(".configuration");

                using (var stream = file.OpenWrite())
                {
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        using (var jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            jsonSerializer.Serialize(jsonWriter, configuration);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SquidexException("Failed to save configuration file.", ex);
            }
        }

        public void Upsert(string config, ConfiguredApp appConfig)
        {
            if (string.IsNullOrWhiteSpace(appConfig.ServiceUrl))
            {
                appConfig.ServiceUrl = CloudUrl;
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
                throw new SquidexException("App config with the name does not exist.");
            }

            configuration.CurrentApp = entry;

            Save();
        }

        public void Remove(string entry)
        {
            if (!configuration.Apps.ContainsKey(entry))
            {
                throw new SquidexException("App config with the name does not exist.");
            }

            configuration.Apps.Remove(entry);

            if (configuration.CurrentApp == entry)
            {
                configuration.CurrentApp = configuration.Apps.FirstOrDefault().Key;
            }

            Save();
        }

        public void UseAppInSession(string entry)
        {
            sessionApp = entry;
        }

        public (string App, SquidexClientManager Client) GetClient()
        {
            if (!string.IsNullOrWhiteSpace(sessionApp) && configuration.Apps.TryGetValue(sessionApp, out var app))
            {
                var authenticator = new Authenticator(app.ServiceUrl, app.ClientId, app.ClientSecret);

                return (app.Name, new SquidexClientManager(app.ServiceUrl, app.Name, authenticator));
            }

            if (!string.IsNullOrWhiteSpace(configuration.CurrentApp) && configuration.Apps.TryGetValue(configuration.CurrentApp, out app))
            {
                var authenticator = new Authenticator(app.ServiceUrl, app.ClientId, app.ClientSecret);

                return (app.Name, new SquidexClientManager(app.ServiceUrl, app.Name, authenticator));
            }

            throw new SquidexException("Cannot find valid configuration.");
        }

        public Configuration GetConfiguration()
        {
            return configuration;
        }
    }
}
