// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using Squidex.CLI.Commands.Implementation;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Configuration
{
    public sealed class Session : ISession
    {
        private readonly SquidexClientManager clientManager;
        private readonly Dictionary<string, IContentsClient<DynamicContent, DynamicData>> contents = new Dictionary<string, IContentsClient<DynamicContent, DynamicData>>();
        private IAppsClient apps;
        private IAssetsClient assets;
        private IBackupsClient backups;
        private ISchemasClient schemas;
        private IExtendableRulesClient rules;

        public string App { get; }

        public DirectoryInfo WorkingDirectory { get; }

        public IAppsClient Apps
        {
            get => apps ??= clientManager.CreateAppsClient();
        }

        public IAssetsClient Assets
        {
            get => assets ??= clientManager.CreateAssetsClient();
        }

        public IBackupsClient Backups
        {
            get => backups ??= clientManager.CreateBackupsClient();
        }

        public IExtendableRulesClient Rules
        {
            get => rules ??= clientManager.CreateExtendableRulesClient();
        }

        public ISchemasClient Schemas
        {
            get => schemas ??= clientManager.CreateSchemasClient();
        }

        public string ClientId
        {
            get => clientManager.Options.ClientId;
        }

        public string ClientSecret
        {
            get => clientManager.Options.ClientSecret;
        }

        public Session(string app, DirectoryInfo workingDirectory, SquidexClientManager clientManager)
        {
            App = app;

            WorkingDirectory = workingDirectory;

            this.clientManager = clientManager;
        }

        public IContentsClient<TEntity, TData> Contents<TEntity, TData>(string schemaName) where TEntity : Content<TData> where TData : class, new()
        {
            return clientManager.CreateContentsClient<TEntity, TData>(schemaName);
        }

        public IContentsClient<DynamicContent, DynamicData> Contents(string schemaName)
        {
            if (!contents.TryGetValue(schemaName, out var client))
            {
                client = clientManager.CreateDynamicContentsClient(schemaName);

                contents[schemaName] = client;
            }

            return client;
        }
    }
}
