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
                            return jsonSerializer.Deserialize<Configuration>(jsonReader);
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

        public void Upsert(string app, ConfiguredApp appConfig)
        {
            if (string.IsNullOrWhiteSpace(appConfig.ServiceUrl))
            {
                appConfig.ServiceUrl = CloudUrl;
            }

            configuration.Apps[app] = appConfig;

            if (string.IsNullOrWhiteSpace(configuration.CurrentApp))
            {
                configuration.CurrentApp = app;
            }

            Save();
        }

        public void UseApp(string app)
        {
            if (!configuration.Apps.ContainsKey(app))
            {
                throw new SquidexException("App with the name does not exist.");
            }

            configuration.CurrentApp = app;

            Save();
        }

        public void Remove(string app)
        {
            if (!configuration.Apps.ContainsKey(app))
            {
                throw new SquidexException("App with the name does not exist.");
            }

            configuration.Apps.Remove(app);

            if (configuration.CurrentApp == app)
            {
                configuration.CurrentApp = configuration.Apps.FirstOrDefault().Key;
            }

            Save();
        }

        public SquidexClientManager GetClient()
        {
            if (!string.IsNullOrWhiteSpace(configuration.CurrentApp) && configuration.Apps.TryGetValue(configuration.CurrentApp, out var app))
            {
                return new SquidexClientManager(app.ServiceUrl, configuration.CurrentApp, new Authenticator(app.ServiceUrl, app.ClientId, app.ClientSecret));
            }

            throw new SquidexException("Cannot find valid configuration.");
        }

        public Configuration GetConfiguration()
        {
            return configuration;
        }
    }
}
