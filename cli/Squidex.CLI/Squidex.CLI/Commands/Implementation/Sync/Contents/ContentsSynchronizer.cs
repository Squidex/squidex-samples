﻿// ==========================================================================
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
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public sealed class ContentsSynchronizer : ISynchronizer
    {
        private const string Ref = "../__json/contents";
        private readonly ILogger log;

        public string Name => "contents";

        public ContentsSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public Task CleanupAsync(DirectoryInfo directoryInfo)
        {
            foreach (var file in GetFiles(directoryInfo))
            {
                file.Delete();
            }

            return Task.CompletedTask;
        }

        public async Task ExportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var schemas = await session.Schemas.GetSchemasAsync(session.App);

            var context = QueryContext.Default.Unpublished().IgnoreFallback();

            foreach (var schema in schemas.Items)
            {
                var client = session.Contents(schema.Name);

                var contents = new List<ContentModel>();
                var contentBatch = 0;

                Task SaveAsync()
                {
                    var model = new ContentsModel
                    {
                        Contents = contents
                    };

                    return log.DoSafeAsync($"Exporting {schema.Name} ({contentBatch})", async () =>
                    {
                        await jsonHelper.WriteWithSchema(directoryInfo, $"contents/{schema.Name}{contentBatch}.json", model, Ref);
                    });
                }

                await client.GetAllAsync(50, async content =>
                {
                    contents.Add(content.ToModel(schema.Name));

                    if (contents.Count > 50)
                    {
                        await SaveAsync();

                        contents.Clear();
                        contentBatch++;
                    }
                }, context);

                if (contents.Count > 0)
                {
                    await SaveAsync();
                }
            }
        }

        public async Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var models =
                GetFiles(directoryInfo)
                    .Select(x => (x, jsonHelper.Read<ContentsModel>(x, log)));

            foreach (var (file, model) in models)
            {
                if (model?.Contents?.Count > 0)
                {
                    model.Clear(options.Languages);

                    var client = session.Contents(model.Contents.First().Schema);

                    var request = new BulkUpdate
                    {
                        OptimizeValidation = true,
                        DoNotScript = true,
                        DoNotValidate = false,
                        DoNotValidateWorkflow = true,
                        Jobs = model.Contents.Select(x => x.ToJob()).ToList()
                    };

                    var contentIdAssigned = false;
                    var contentIndex = 0;

                    var results = await client.BulkUpdateAsync(request);

                    foreach (var content in model.Contents)
                    {
                        var result = results.FirstOrDefault(x => x.JobIndex == contentIndex);

                        log.StepStart($"Upserting #{contentIndex}");

                        if (result?.Error != null)
                        {
                            log.StepFailed(result.Error.ToString());
                        }
                        else if (result?.ContentId != null)
                        {
                            if (string.IsNullOrWhiteSpace(content.Id))
                            {
                                content.Id = result.ContentId;
                                contentIdAssigned = true;
                            }

                            log.StepSuccess();
                        }
                        else
                        {
                            log.StepSkipped("Unknown Reason");
                        }

                        contentIndex++;
                    }

                    if (contentIdAssigned)
                    {
                        await log.DoSafeAsync($"Saving {file.Name}", async () =>
                        {
                            await jsonHelper.WriteWithSchema(directoryInfo, file.FullName, model, Ref);
                        });
                    }
                }
            }
        }

        private static IEnumerable<FileInfo> GetFiles(DirectoryInfo directoryInfo)
        {
            foreach (var file in directoryInfo.GetFiles("contents/*.json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
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
                        Data = new DynamicData
                        {
                            ["id"] = new JObject
                            {
                                ["iv"] = 1
                            },
                            ["text"] = new JObject
                            {
                                ["iv"] = "Hello Squidex"
                            }
                        },
                        Status = "Published"
                    }
                }
            };

            await jsonHelper.WriteWithSchema(directoryInfo, "contents/__contents.json", sample, Ref);
        }
    }
}
