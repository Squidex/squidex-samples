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
using Newtonsoft.Json.Linq;
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

        public async Task ExportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var schemas = await session.Schemas.GetSchemasAsync(session.App);

            var context = QueryContext.Default.Unpublished().IgnoreFallback();

            foreach (var schema in schemas.Items)
            {
                var client = session.Contents(schema.Name);

                var contents = new List<ContentModel>();
                var contentBatch = 0;

                await client.GetAllAsync(50, async content =>
                {
                    contents.Add(new ContentModel
                    {
                        Id = content.Id,
                        Data = content.Data,
                        Status = content.Status,
                        Schema = schema.Name
                    });

                    if (contents.Count > 50)
                    {
                        var model = new ContentsModel
                        {
                            Contents = contents
                        };

                        await log.DoSafeAsync($"Exporting {schema.Name} ({contentBatch})", async () =>
                        {
                            await jsonHelper.WriteWithSchema(directoryInfo, $"contents/{schema.Name}{contentBatch}.json", model, "../__json/contents");
                        });

                        contents.Clear();
                        contentBatch++;
                    }
                }, context);

                if (contents.Count > 0)
                {
                    var model = new ContentsModel
                    {
                        Contents = contents
                    };

                    await log.DoSafeAsync($"Exporting {schema.Name} ({contentBatch})", async () =>
                    {
                        await jsonHelper.WriteWithSchema(directoryInfo, $"contents/{schema.Name}{contentBatch}.json", model, "../__json/contents");
                    });
                }
            }
        }

        public async Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            foreach (var file in GetContentFiles(directoryInfo))
            {
                ContentsModel model = null;

                await log.DoSafeAsync($"Reading file {file.Name}", () =>
                {
                    model = ReadContents(jsonHelper, file);

                    return Task.CompletedTask;
                });

                if (model?.Contents?.Count > 0)
                {
                    if (options.Languages?.Length > 0)
                    {
                        var allowedLanguages = options.Languages.ToHashSet();

                        var toClear = new List<string>();

                        foreach (var content in model.Contents)
                        {
                            foreach (var field in content.Data.Values)
                            {
                                foreach (var language in field.Children<JProperty>().Select(x => x.Name))
                                {
                                    if (language != "iv" && !allowedLanguages.Contains(language))
                                    {
                                        toClear.Add(language);
                                    }
                                }

                                if (toClear.Count > 0)
                                {
                                    foreach (var language in toClear)
                                    {
                                        field.Remove(language);
                                    }

                                    toClear.Clear();
                                }
                            }
                        }
                    }

                    var client = session.Contents(model.Contents.First().Schema);

                    var request = new BulkUpdate
                    {
                        OptimizeValidation = true,
                        DoNotScript = true,
                        DoNotValidate = false,
                        DoNotValidateWorkflow = true,
                        Jobs = model.Contents.Select(x => new BulkUpdateJob
                        {
                            Id = x.Id,
                            Data = x.Data,
                            Schema = x.Schema,
                            Status = x.Status,
                            Type = BulkUpdateType.Upsert,
                        }).ToList(),
                    };

                    var results = await client.BulkUpdateAsync(request);

                    var i = 0;

                    foreach (var result in results)
                    {
                        var content = model.Contents[i];

                        log.StepStart(i.ToString());

                        if (result.Error != null)
                        {
                            log.StepFailed(result.Error.ToString());
                        }
                        else if (result.ContentId != null)
                        {
                            log.StepSuccess();
                        }
                        else
                        {
                            log.StepSkipped("Unknown Reason");
                        }

                        i++;
                    }
                }
            }
        }

        private ContentsModel ReadContents(JsonHelper jsonHelper, FileInfo file)
        {
            var result = jsonHelper.Read<ContentsModel>(file, log);

            var index = 0;

            foreach (var content in result.Contents)
            {
                content.File = file.Name;
                content.Ref = index.ToString();

                index++;
            }

            return result;
        }

        private IEnumerable<FileInfo> GetContentFiles(DirectoryInfo directoryInfo)
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
                        Status = "Published",
                        Schema = "my-schema",
                        Id = Guid.NewGuid().ToString(),
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
                        }
                    }
                }
            };

            await jsonHelper.WriteWithSchema(directoryInfo, "contents/__contents.json", sample, "../__json/contents");
        }
    }
}
