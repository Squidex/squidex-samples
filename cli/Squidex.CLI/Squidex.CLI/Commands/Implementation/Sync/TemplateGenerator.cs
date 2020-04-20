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
    public class TemplateGenerator
    {
        private readonly DirectoryInfo directoryInfo;
        private readonly IEnumerable<ISynchronizer> synchronizers;

        public TemplateGenerator(string path, IEnumerable<ISynchronizer> synchronizers)
        {
            this.synchronizers = synchronizers;

            directoryInfo = Directory.CreateDirectory(path);
        }

        public async Task GenerateAsync()
        {
            var jsonHelper = new JsonHelper();

            foreach (var synchronizer in synchronizers.OrderBy(x => x.Order))
            {
                await synchronizer.GenerateSchemaAsync(directoryInfo, jsonHelper);
            }
        }
    }
}
