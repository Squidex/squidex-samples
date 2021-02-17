// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.ImExport
{
    public static class ExportHelper
    {
        public static async Task ExportAsync(this ISession session, IExportSettings settings, ILogger log, Func<DynamicContent, Task> handler)
        {
            var ctx = QueryContext.Default.Unpublished(settings.Unpublished);

            var contents = session.Contents(settings.Schema);

            var total = 0L;
            var totalRead = 0;
            var currentPage = 0;

            var handled = new HashSet<string>();

            using (var logLine = log.WriteSameLine())
            {
                do
                {
                    var query = new ContentQuery
                    {
                        Filter = settings.Filter,
                        OrderBy = settings.OrderBy,
                        Search = settings.FullText,
                        Skip = currentPage * 100,
                        Top = 100
                    };

                    var content = await contents.GetAsync(query, ctx);

                    total = content.Total;

                    if (content.Items.Count == 0)
                    {
                        break;
                    }

                    foreach (var entity in content.Items)
                    {
                        if (handled.Add(entity.Id))
                        {
                            totalRead++;

                            await handler(entity);

                            logLine.WriteLine("> Exported: {0} of {1}.", totalRead, total);
                        }
                    }

                    currentPage++;
                }
                while (totalRead < total);
            }

            log.WriteLine("> Exported: {0} of {1}. Completed.", totalRead, total);
        }
    }
}
