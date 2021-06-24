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

        public Task CleanupAsync(IFileSystem fs)
        {
            foreach (var file in GetFiles(fs))
            {
                file.Delete();
            }

            return Task.CompletedTask;
        }

        public async Task ExportAsync(IFileSystem fs, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var downloadPipeline = new DownloadPipeline(session, log, fs);

            var assets = new List<AssetModel>();
            var assetBatch = 0;

            async Task SaveAsync()
            {
                var model = new AssetsModel
                {
                    Assets = assets
                };

                await log.DoSafeAsync($"Exporting Assets ({assetBatch})", async () =>
                {
                    await jsonHelper.WriteWithSchema(fs, new FilePath("assets", $"{assetBatch}.json"), model, Ref);
                });
            }

            var tree = new FolderTree(session);

            await session.Assets.GetAllAsync(session.App, async asset =>
            {
                assets.Add(await asset.ToModelAsync(tree));

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

        public async Task ImportAsync(IFileSystem fs, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var models =
                GetFiles(fs)
                    .Select(x => (x, jsonHelper.Read<AssetsModel>(x, log)));

            var tree = new FolderTree(session);

            foreach (var (_, model) in models)
            {
                if (model?.Assets?.Count > 0)
                {
                    var uploader = new UploadPipeline(session, log, fs);

                    await uploader.UploadAsync(model.Assets);
                    await uploader.CompleteAsync();

                    var request = new BulkUpdateAssetsDto();

                    foreach (var asset in model.Assets)
                    {
                        var parentId = await tree.GetIdAsync(asset.FolderPath);

                        request.Jobs.Add(asset.ToMoveJob(parentId));
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

        private static IEnumerable<IFile> GetFiles(IFileSystem fs)
        {
            foreach (var file in fs.GetFiles(new FilePath("assets"), ".json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }
        }

        public async Task GenerateSchemaAsync(IFileSystem fs, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<AssetsModel>(fs, new FilePath("assets.json"));

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

            await jsonHelper.WriteWithSchema(fs, new FilePath("assets", "__asset.json"), sample, Ref);
        }
    }
}
