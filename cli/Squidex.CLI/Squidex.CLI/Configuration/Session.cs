// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
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

        public IAppsClient Apps
        {
            get
            {
                apps ??= apps = clientManager.CreateAppsClient();

                return apps;
            }
        }

        public IAssetsClient Assets
        {
            get
            {
                assets ??= assets = clientManager.CreateAssetsClient();

                return assets;
            }
        }

        public IBackupsClient Backups
        {
            get
            {
                backups ??= backups = clientManager.CreateBackupsClient();

                return backups;
            }
        }

        public IExtendableRulesClient Rules
        {
            get
            {
                rules ??= rules = clientManager.CreateExtendableRulesClient();

                return rules;
            }
        }

        public ISchemasClient Schemas
        {
            get
            {
                schemas ??= schemas = clientManager.CreateSchemasClient();

                return schemas;
            }
        }

        public string ClientId => clientManager.Options.ClientId;

        public Session(string app, SquidexClientManager clientManager)
        {
            App = app;

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
