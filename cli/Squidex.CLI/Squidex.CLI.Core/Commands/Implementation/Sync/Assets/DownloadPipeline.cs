// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks.Dataflow;
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets;

public sealed class DownloadPipeline
{
    private readonly ITargetBlock<AssetDto> pipelineStart;
    private readonly IDataflowBlock pipelineEnd;

    public Func<AssetDto, FilePath> FilePathProvider { get; set; }

    public Func<AssetDto, Task<FilePath>> FilePathProviderAsync { get; set; }

    public DownloadPipeline(ISession session, ILogger log, IFileSystem fs)
    {
        var fileNameStep = new TransformBlock<AssetDto, (AssetDto, FilePath)>(async asset =>
        {
            FilePath path;

            if (FilePathProvider != null)
            {
                path = FilePathProvider(asset);
            }
            else if (FilePathProviderAsync != null)
            {
                path = await FilePathProviderAsync(asset);
            }
            else
            {
                path = new FilePath(asset.Id);
            }

            return (asset, path);
        },
        new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 1,
            MaxMessagesPerTask = 1,
            BoundedCapacity = 1
        });

        var maxDegreeOfParallelism = fs.CanAccessInParallel ? Environment.ProcessorCount * 2 : 1;

        var downloadStep = new ActionBlock<(AssetDto, FilePath)>(async item =>
        {
            var (asset, path) = item;

            var process = $"Downloading {path}";
            try
            {
                var assetFile = fs.GetFile(path);
                var assetHash = assetFile.GetFileHash(asset);

                if (assetHash == null || !string.Equals(asset.FileHash, assetHash, StringComparison.Ordinal))
                {
                    var response = await GetAssetAsync(session, asset);

                    await using (response.Stream)
                    {
                        await using (var stream = assetFile.OpenWrite())
                        {
                            await response.Stream.CopyToAsync(stream);
                        }
                    }

                    log.ProcessCompleted(process);
                }
                else
                {
                    log.ProcessSkipped(process, "Same hash.");
                }
            }
            catch (Exception ex)
            {
                log.ProcessFailed(process, ex);
            }
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism,
            MaxMessagesPerTask = 1,
            BoundedCapacity = maxDegreeOfParallelism * 2
        });

        fileNameStep.BidirectionalLinkTo(downloadStep);

        pipelineStart = fileNameStep;
        pipelineEnd = downloadStep;
    }

    private static async Task<FileResponse> GetAssetAsync(ISession session, AssetDto asset)
    {
        try
        {
            return await session.Client.Assets.GetAssetContentBySlugAsync(asset.Id, string.Empty);
        }
        catch (SquidexException ex) when (ex.StatusCode == 404)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            return await session.Client.Assets.GetAssetContentAsync(asset.Id);
#pragma warning restore CS0612 // Type or member is obsolete
        }
    }

    public Task DownloadAsync(AssetDto asset)
    {
        return pipelineStart.SendAsync(asset);
    }

    public Task CompleteAsync()
    {
        pipelineStart.Complete();

        return pipelineEnd.Completion;
    }
}
