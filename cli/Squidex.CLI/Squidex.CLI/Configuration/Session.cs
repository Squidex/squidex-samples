// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using Squidex.CLI.Commands;
using Squidex.CLI.Commands.Implementation;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Configuration
{
    public sealed class Session : ISession
    {
        private readonly SquidexClientManager clientManager;
        private readonly Dictionary<string, IContentsClient<DummyEntity, DummyData>> contents = new Dictionary<string, IContentsClient<DummyEntity, DummyData>>();
        private IAppsClient apps;
        private IBackupsClient backups;
        private ISchemasClient schemas;

        public string App { get; }

        public IAppsClient Apps
        {
            get
            {
                apps ??= (apps = clientManager.CreateAppsClient());

                return apps;
            }
        }

        public IBackupsClient Backups
        {
            get
            {
                backups ??= (backups = clientManager.CreateBackupsClient());

                return backups;
            }
        }

        public ISchemasClient Schemas
        {
            get
            {
                schemas ??= (schemas = clientManager.CreateSchemasClient());

                return schemas;
            }
        }

        public string ClientId => clientManager.Options.ClientId;

        public Session(string app, SquidexClientManager clientManager)
        {
            App = app;

            this.clientManager = clientManager;
        }

        public IContentsClient<DummyEntity, DummyData> Contents(string schema)
        {
            if (!contents.TryGetValue(schema, out var client))
            {
                client = clientManager.CreateContentsClient<DummyEntity, DummyData>(schema);

                contents[schema] = client;
            }

            return client;
        }
    }
}
