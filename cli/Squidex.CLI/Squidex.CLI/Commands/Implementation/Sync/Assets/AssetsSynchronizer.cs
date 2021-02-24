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
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets
{
    public sealed class AssetsSynchronizer : ISynchronizer
    {
        private const string Ref = "../__json/assets";
        private readonly ILogger log;

        public int Order => -1000;

        public string Name => "Assets";

        public AssetsSynchronizer(ILogger log)
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
            var downloadPipeline = new DownloadPipeline(session, log, directoryInfo);

            var assets = new List<AssetModel>();
            var assetBatch = 0;

            async Task SaveAsync()
            {
                var model = new AssetsModel
                {
                    Assets = assets
                };

                await log.DoSafeAsync($"Exporting Assets ({assetBatch})", (Func<Task>)(async () =>
                {
                    await jsonHelper.WriteWithSchema(directoryInfo, $"assets/{assetBatch}.json", model, (string)AssetsSynchronizer.Ref);
                }));
            }

            var pathCache = new Dictionary<string, string>();

            await session.Assets.GetAllAsync(session.App, async asset =>
            {
                assets.Add(await asset.ToModelAsync(session, pathCache));

                if (assets.Count > 50)
                {
                    await SaveAsync();

                    assets.Clear();
                    assetBatch++;
                }

                await downloadPipeline.DownloadAsync(asset);
            });

            if (assets.Count > 0)
            {
                await SaveAsync();
            }

            await downloadPipeline.CompleteAsync();
        }

        public async Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var models =
                GetFiles(directoryInfo)
                    .Select(x => (x, jsonHelper.Read<AssetsModel>(x, log)));

            foreach (var (file, model) in models)
            {
                if (model?.Assets?.Count > 0)
                {
                    var uploader = new UploadPipeline(session, log, directoryInfo);

                    await uploader.UploadAsync(model.Assets);
                    await uploader.CompleteAsync();

                    var request = new BulkUpdateAssetsDto();

                    foreach (var asset in model.Assets)
                    {
                        request.Jobs.Add(asset.ToMoveJob());
                        request.Jobs.Add(asset.ToAnnotateJob());
                    }

                    var assetIndex = 0;

                    var results = await session.Assets.BulkUpdateAssetsAsync(session.App, request);

                    foreach (var asset in model.Assets)
                    {
                        // We create wo commands per asset.
                        var result1 = results.FirstOrDefault(x => x.JobIndex == (assetIndex * 2));
                        var result2 = results.FirstOrDefault(x => x.JobIndex == (assetIndex * 2) + 1);

                        log.StepStart($"Upserting #{assetIndex}");

                        if (result1?.Error != null)
                        {
                            log.StepFailed(result1.Error.ToString());
                        }
                        else if (result2?.Error != null)
                        {
                            log.StepFailed(result2.Error.ToString());
                        }
                        else if (result1?.Id != null && result2.Id != null)
                        {
                            log.StepSuccess();
                        }
                        else
                        {
                            log.StepSkipped("Unknown Reason");
                        }

                        assetIndex++;
                    }
                }
            }
        }

        private static IEnumerable<FileInfo> GetFiles(DirectoryInfo directoryInfo)
        {
            foreach (var file in directoryInfo.GetFiles("assets/*.json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }
        }

        public async Task GenerateSchemaAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<AssetsModel>(directoryInfo, "assets.json");

            var sample = new AssetsModel
            {
                Assets = new List<AssetModel>
                {
                    new AssetModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        FileName = "my.file.txt",
                        FileHash = "<Optional Hash>",
                        MimeType = "plain/text"
                    }
                }
            };

            await jsonHelper.WriteWithSchema(directoryInfo, "assets/__asset.json", sample, Ref);
        }
    }
}
