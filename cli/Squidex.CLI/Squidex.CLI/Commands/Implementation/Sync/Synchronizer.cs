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
        private readonly ILogger log;
        private readonly IEnumerable<ISynchronizer> synchronizers;

        public Synchronizer(IEnumerable<ISynchronizer> synchronizers, ILogger log)
        {
            this.synchronizers = synchronizers;

            this.log = log;
        }

        public async Task SynchronizeAsync(string path, SyncOptions options, ISession session)
        {
            var directoryInfo = Directory.CreateDirectory(path);

            WriteSummary(directoryInfo);

            var jsonHelper = new JsonHelper();

            var step = 1;

            foreach (var synchronizer in synchronizers.OrderBy(x => x.Order))
            {
                log.WriteLine();
                log.WriteLine("--------------------------------------------------------");
                log.WriteLine("* STEP {0} of {1}: Synchronizing {2} started", step, synchronizers.Count(), synchronizer.Name);
                log.WriteLine();

                await synchronizer.SynchronizeAsync(directoryInfo, jsonHelper, options, session);

                log.WriteLine();
                log.WriteLine("* STEP {0} of {1}: Synchronizing {2} completed", step, synchronizers.Count(), synchronizer.Name);
                log.WriteLine("--------------------------------------------------------");

                step++;
            }
        }

        private void WriteSummary(DirectoryInfo directoryInfo)
        {
            log.WriteLine("Synchronizing from {0}", directoryInfo.FullName);
            log.WriteLine();
            log.WriteLine("Executing the following steps");

            var step = 1;

            foreach (var synchronizer in synchronizers.OrderBy(x => x.Order))
            {
                log.WriteLine("* STEP {0} of {1}: {2}", step, synchronizers.Count(), synchronizer.Name);

                step++;
            }
        }

        public async Task GenerateTemplateAsync(string path)
        {
            var directoryInfo = Directory.CreateDirectory(path);

            var jsonHelper = new JsonHelper();

            foreach (var synchronizer in synchronizers.OrderBy(x => x.Order))
            {
                await synchronizer.GenerateSchemaAsync(directoryInfo, jsonHelper);
            }
        }
    }
}
