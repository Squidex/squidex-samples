// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Squidex.CLI.Commands.Implementation.FileSystem;

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
            using (var fs = FileSystems.Create(path))
            {
                if (!fs.CanWrite)
                {
                    Console.WriteLine("ERROR: Cannot write to the file system.");
                    return;
                }

                var selectedSynchronizers = GetSynchronizers(options.Targets);
                var selectedCount = selectedSynchronizers.Count;

                WriteSummary(fs, selectedSynchronizers);

                var sync = new SyncService(fs);

                foreach (var synchronizer in selectedSynchronizers)
                {
                    await synchronizer.GenerateSchemaAsync(sync);
                }

                await selectedSynchronizers.Foreach(async (synchronizer, step) =>
                {
                    log.WriteLine();
                    log.WriteLine("--------------------------------------------------------");
                    log.WriteLine("* STEP {0} of {1}: Exporting {2} started", step, selectedCount, synchronizer.Name);
                    log.WriteLine();

                    await synchronizer.CleanupAsync(fs);
                    await synchronizer.ExportAsync(sync, options, session);

                    log.WriteLine();
                    log.WriteLine("* STEP {0} of {1}: Exporting {2} completed", step, selectedSynchronizers.Count, synchronizer.Name);
                    log.WriteLine("--------------------------------------------------------");
                });
            }
        }

        public async Task ImportAsync(string path, SyncOptions options, ISession session)
        {
            using (var fs = FileSystems.Create(path))
            {
                var selectedSynchronizers = GetSynchronizers(options.Targets);
                var selectedCount = selectedSynchronizers.Count;

                WriteSummary(fs, selectedSynchronizers);

                var sync = new SyncService(fs);

                await selectedSynchronizers.Foreach(async (synchronizer, step) =>
                {
                    log.WriteLine();
                    log.WriteLine("--------------------------------------------------------");
                    log.WriteLine("* STEP {0} of {1}: Importing {2} started", step, selectedCount, synchronizer.Name);
                    log.WriteLine();

                    await synchronizer.ImportAsync(sync, options, session);

                    log.WriteLine();
                    log.WriteLine("* STEP {0} of {1}: Importing {2} completed", step, selectedCount, synchronizer.Name);
                    log.WriteLine("--------------------------------------------------------");
                });
            }
        }

        private void WriteSummary(IFileSystem fs, List<ISynchronizer> selectedSynchronizers)
        {
            log.WriteLine("Synchronizing from {0}", fs.FullName);
            log.WriteLine();
            log.WriteLine("Executing the following steps");

            var selectedCount = selectedSynchronizers.Count;

            selectedSynchronizers.Foreach((synchronizer, step) =>
            {
                log.WriteLine("* STEP {0} of {1}: {2}", step, selectedCount, synchronizer.Name);
            });
        }

        public async Task GenerateTemplateAsync(string path)
        {
            using (var fs = FileSystems.Create(path))
            {
                if (!fs.CanWrite)
                {
                    Console.WriteLine("ERROR: Cannot write to the file system.");
                    return;
                }

                var sync = new SyncService(fs);

                foreach (var synchronizer in GetSynchronizers())
                {
                    await synchronizer.GenerateSchemaAsync(sync);
                }
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
