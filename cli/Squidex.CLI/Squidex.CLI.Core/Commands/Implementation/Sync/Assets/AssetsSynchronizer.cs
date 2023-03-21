// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets;

public sealed class AssetsSynchronizer : ISynchronizer
{
    private const string Ref = "../__json/assets";
    private readonly ILogger log;

    public int Order => -1000;

    public string Name => "Assets";

    public string Description => "Synchronizes all assets and creates asset folders if they do not exist yet.";

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

    public async Task ExportAsync(ISyncService sync, SyncOptions options, ISession session)
    {
        var downloadPipeline = new DownloadPipeline(session, log, sync.FileSystem)
        {
            FilePathProvider = asset => asset.Id.GetBlobPath()
        };

        try
        {
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
                    await sync.WriteWithSchema(new FilePath("assets", $"{assetBatch}.json"), model, Ref);
                });
            }

            await session.Client.Assets.GetAllAsync(async asset =>
            {
                var model = asset.ToModel();

                model.FolderPath = await sync.Folders.GetPathAsync(asset.ParentId);

                assets.Add(model);

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
        }
        finally
        {
            await downloadPipeline.CompleteAsync();
        }
    }

    public Task DescribeAsync(ISyncService sync, MarkdownWriter writer)
    {
        var models =
            GetFiles(sync.FileSystem)
                .Select(x => (x, sync.Read<AssetsModel>(x, log)));

        writer.Paragraph($"{models.SelectMany(x => x.Item2.Assets).Count()} asset(s).");

        return Task.CompletedTask;
    }

    public async Task ImportAsync(ISyncService sync, SyncOptions options, ISession session)
    {
        var models =
            GetFiles(sync.FileSystem)
                .Select(x => (x, sync.Read<AssetsModel>(x, log)));

        var batchIndex = 0;

        foreach (var (_, model) in models)
        {
            if (model?.Assets?.Count > 0)
            {
                var uploader = new UploadPipeline(session, log, sync.FileSystem)
                {
                    FilePathProvider = asset => asset.Id.GetBlobPath()
                };

                try
                {
                    foreach (var asset in model.Assets)
                    {
                        await uploader.UploadAsync(asset);
                    }
                }
                finally
                {
                    await uploader.CompleteAsync();
                }

                var request = new BulkUpdateAssetsDto();

                foreach (var asset in model.Assets)
                {
                    var parentId = await sync.Folders.GetIdAsync(asset.FolderPath);

                    request.Jobs.Add(asset.ToMove(parentId));
                    request.Jobs.Add(asset.ToAnnotate());
                }

                var assetIndex = 0;

                var results = await session.Client.Assets.BulkUpdateAssetsAsync(request);

                foreach (var asset in model.Assets)
                {
                    // We create wo commands per asset.
                    var result1 = results.FirstOrDefault(x => x.JobIndex == (assetIndex * 2));
                    var result2 = results.FirstOrDefault(x => x.JobIndex == (assetIndex * 2) + 1);

                    log.StepStart($"Upserting #{batchIndex}/{assetIndex}");

                    if (result1?.Error != null)
                    {
                        log.StepFailed(result1.Error.ToString());
                    }
                    else if (result2?.Error != null)
                    {
                        log.StepFailed(result2.Error.ToString());
                    }
                    else if (result1?.Id != null && result2?.Id != null)
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

            batchIndex++;
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

    public async Task GenerateSchemaAsync(ISyncService sync)
    {
        await sync.WriteJsonSchemaAsync<AssetsModel>(new FilePath("assets.json"));

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

        await sync.WriteWithSchema(new FilePath("assets", "__asset.json"), sample, Ref);
    }
}
