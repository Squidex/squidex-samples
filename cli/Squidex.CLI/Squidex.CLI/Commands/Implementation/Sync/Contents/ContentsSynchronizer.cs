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
using Newtonsoft.Json.Linq;
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public sealed class ContentsSynchronizer : ISynchronizer
    {
        private const string Ref = "../__json/contents";
        private readonly ILogger log;

        public string Name => "Contents";

        public ContentsSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public Task CleanupAsync(IFileSystem fs)
        {
            foreach (var file in GetFiles(fs))
            {
                file.Delete();
            }

            return Task.CompletedTask;
        }

        public async Task ExportAsync(ISyncService sync, SyncOptions options, ISession session)
        {
            var schemas = await session.Schemas.GetSchemasAsync(session.App);
            var schemaMap = schemas.Items.ToDictionary(x => x.Id, x => x.Name);

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
                        await sync.WriteWithSchema(new FilePath("contents", schema.Name, $"{contentBatch}.json"), model, Ref);
                    });
                }

                await client.GetAllAsync(async content =>
                {
                    content.MapComponents(schemaMap);

                    contents.Add(content.ToModel(schema.Name));

                    if (contents.Count > 50)
                    {
                        await SaveAsync();

                        contents.Clear();
                        contentBatch++;
                    }
                }, context: context);

                if (contents.Count > 0)
                {
                    await SaveAsync();
                }
            }
        }

        public async Task ImportAsync(ISyncService sync, SyncOptions options, ISession session)
        {
            var models =
                GetFiles(sync.FileSystem)
                    .Select(x => (x, sync.Read<ContentsModel>(x, log)));

            var schemas = await session.Schemas.GetSchemasAsync(session.App);
            var schemaMap = schemas.Items.ToDictionary(x => x.Name, x => x.Id);

            foreach (var (file, model) in models)
            {
                if (model?.Contents?.Count > 0)
                {
                    model.Clear(options.Languages);

                    foreach (var content in model.Contents)
                    {
                        content.MapComponents(schemaMap);
                    }

                    var client = session.Contents(model.Contents[0].Schema);

                    var request = new BulkUpdate
                    {
                        OptimizeValidation = true,
                        DoNotScript = true,
                        DoNotValidate = false,
                        DoNotValidateWorkflow = true,
                        Jobs = model.Contents.Select(x => x.ToJob(schemas)).ToList()
                    };

                    var contentIdAssigned = false;
                    var contentIndex = 0;

                    var results = await client.BulkUpdateAsync(request);

                    foreach (var content in model.Contents)
                    {
                        var result = results.Find(x => x.JobIndex == contentIndex);

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
                            await sync.WriteWithSchema(file, model, Ref);
                        });
                    }
                }
            }
        }

        private static IEnumerable<IFile> GetFiles(IFileSystem fs)
        {
            foreach (var file in fs.GetFiles(new FilePath("contents"), ".json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }
        }

        public async Task GenerateSchemaAsync(ISyncService sync)
        {
            await sync.WriteJsonSchemaAsync<ContentsModel>(new FilePath("contents.json"));

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

            await sync.WriteWithSchema(new FilePath("contents", "__contents.json"), sample, Ref);
        }
    }
}
