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
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public sealed class ContentsSynchronizer : ISynchronizer
    {
        private readonly ILogger log;

        public string Name => "contents";

        public ContentsSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public Task ExportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            log.WriteLine("Not supported");

            return Task.CompletedTask;
        }

        public async Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            foreach (var file in directoryInfo.GetFiles("contents\\*.json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    ContentsModel contents = null;

                    await log.DoSafeAsync($"Reading file {file.Name}", () =>
                    {
                        contents = jsonHelper.Read<ContentsModel>(file, log);

                        return Task.CompletedTask;
                    });

                    if (contents == null)
                    {
                        continue;
                    }

                    foreach (var schema in contents.Contents.GroupBy(x => x.Schema))
                    {
                        var client = session.Contents(schema.Key);

                        List<BulkResult> results = null;

                        await log.DoSafeAsync("Importing contents", async () =>
                        {
                            var request = new BulkUpdate
                            {
                                Jobs = schema.Select(x => new BulkUpdateJob { Query = x.Query, Data = x.Data }).ToList()
                            };

                            results = await client.BulkUpdateAsync(request);
                        });

                        if (results != null)
                        {
                            var i = 1;

                            foreach (var result in results)
                            {
                                log.StepStart($"Content #{i}");

                                if (result.ContentId != null)
                                {
                                    log.StepSuccess();
                                }
                                else if (result.Error != null)
                                {
                                    log.StepFailed(result.Error.ToString());
                                }
                                else
                                {
                                    log.StepSkipped("Unknown Reason");
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task GenerateSchemaAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<ContentsModel>(directoryInfo, "contents.json");

            var sample = new ContentsModel
            {
                Contents = new List<ContentModel>
                {
                    new ContentModel
                    {
                        Schema = "my-schema",
                        Query = new
                        {
                            Filter = new
                            {
                                Path = "data.id.iv",
                                Op = "eq",
                                Value = 1
                            }
                        },
                        Data = new
                        {
                            id = new
                            {
                                iv = 1
                            },
                            text = new
                            {
                                iv = "Hello Squidex"
                            }
                        }
                    }
                }
            };

            await jsonHelper.WriteWithSchema(directoryInfo, "contents/__contents.json", sample, "../__json/contents");
        }
    }
}
