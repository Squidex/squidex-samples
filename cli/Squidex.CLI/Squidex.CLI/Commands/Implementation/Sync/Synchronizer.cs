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

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public sealed class Synchronizer
    {
        private readonly DirectoryInfo directoryInfo;
        private readonly ISession session;
        private readonly SyncOptions options;
        private readonly IEnumerable<ISynchronizer> synchronizers;

        public Synchronizer(string path, ISession session, SyncOptions options, IEnumerable<ISynchronizer> synchronizers)
        {
            directoryInfo = Directory.CreateDirectory(path);

            this.options = options;
            this.session = session;
            this.synchronizers = synchronizers;
        }

        public async Task SyncAsync()
        {
            var jsonHelper = new JsonHelper();

            foreach (var synchronizer in synchronizers.OrderBy(x => x.Order))
            {
                await synchronizer.SynchronizeAsync(directoryInfo, jsonHelper, options, session);
            }
        }
    }
}
