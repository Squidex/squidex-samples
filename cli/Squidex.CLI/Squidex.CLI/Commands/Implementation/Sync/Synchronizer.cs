// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
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

        public IEnumerable<string> GetTargets()
        {
            return GetSynchronizers().Select(x => x.Name);
        }

        public async Task ExportAsync(string path, SyncOptions options, ISession session)
        {
            var directoryInfo = Directory.CreateDirectory(path);

            var selectedSynchronizers = GetSynchronizers(options.Targets);

            WriteSummary(directoryInfo, selectedSynchronizers);

            var jsonHelper = new JsonHelper();

            foreach (var synchronizer in selectedSynchronizers)
            {
                await synchronizer.GenerateSchemaAsync(directoryInfo, jsonHelper);
            }

            await selectedSynchronizers.Foreach(async (synchronizer, step) =>
            {
                log.WriteLine();
                log.WriteLine("--------------------------------------------------------");
                log.WriteLine("* STEP {0} of {1}: Exporting {2} started", step, synchronizers.Count(), synchronizer.Name);
                log.WriteLine();

                await synchronizer.CleanupAsync(directoryInfo);
                await synchronizer.ExportAsync(directoryInfo, jsonHelper, options, session);

                log.WriteLine();
                log.WriteLine("* STEP {0} of {1}: Exporting {2} completed", step, synchronizers.Count(), synchronizer.Name);
                log.WriteLine("--------------------------------------------------------");
            });
        }

        public async Task ImportAsync(string path, SyncOptions options, ISession session)
        {
            var directoryInfo = new DirectoryInfo(path);

            var selectedSynchronizers = GetSynchronizers(options.Targets);

            WriteSummary(directoryInfo, selectedSynchronizers);

            var jsonHelper = new JsonHelper();

            await selectedSynchronizers.Foreach(async (synchronizer, step) =>
            {
                log.WriteLine();
                log.WriteLine("--------------------------------------------------------");
                log.WriteLine("* STEP {0} of {1}: Importing {2} started", step, synchronizers.Count(), synchronizer.Name);
                log.WriteLine();

                await synchronizer.ImportAsync(directoryInfo, jsonHelper, options, session);

                log.WriteLine();
                log.WriteLine("* STEP {0} of {1}: Importing {2} completed", step, synchronizers.Count(), synchronizer.Name);
                log.WriteLine("--------------------------------------------------------");
            });
        }

        private void WriteSummary(DirectoryInfo directoryInfo, List<ISynchronizer> selectedSynchronizers)
        {
            log.WriteLine("Synchronizing from {0}", directoryInfo.FullName);
            log.WriteLine();
            log.WriteLine("Executing the following steps");

            selectedSynchronizers.Foreach((synchronizer, step) =>
            {
                log.WriteLine("* STEP {0} of {1}: {2}", step, synchronizers.Count(), synchronizer.Name);
            });
        }

        public async Task GenerateTemplateAsync(string path)
        {
            var directoryInfo = Directory.CreateDirectory(path);

            var jsonHelper = new JsonHelper();

            foreach (var synchronizer in GetSynchronizers())
            {
                await synchronizer.GenerateSchemaAsync(directoryInfo, jsonHelper);
            }
        }

        private List<ISynchronizer> GetSynchronizers(string[] targets = null)
        {
            var selected = synchronizers;

            if (targets?.Length > 0)
            {
                selected = selected.Where(x => targets.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
            }

            return selected.OrderBy(x => x.Order).ToList();
        }
    }
}
