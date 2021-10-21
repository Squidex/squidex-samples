// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets
{
    public sealed class UploadPipeline
    {
        private readonly ITargetBlock<AssetModel> pipelineStart;
        private readonly IDataflowBlock pipelineEnd;

        public Func<AssetModel, FilePath> FilePathProvider { get; set; }

        public Func<AssetModel, Task<FilePath>> FilePathProviderAsync { get; set; }

        public UploadPipeline(ISession session, ILogger log, IFileSystem fs)
        {
            var tree = new FolderTree(session);

            var fileNameStep = new TransformBlock<AssetModel, (AssetModel, FilePath)>(async asset =>
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

            var uploadStep = new ActionBlock<(AssetModel, FilePath)>(async item =>
            {
                var (asset, path) = item;

                var process = $"Uploading {path}";

                try
                {
                    var assetFile = fs.GetFile(path);

                    await using (var stream = assetFile.OpenRead())
                    {
                        var file = new FileParameter(stream, asset.FileName, asset.MimeType);

                        var result = await session.Assets.PostUpsertAssetAsync(session.App, asset.Id, null, true, file);

                        if (string.Equals(asset.FileHash, result.FileHash, StringComparison.Ordinal))
                        {
                            log.ProcessSkipped(process, "Same hash.");
                        }
                        else
                        {
                            log.ProcessCompleted(process);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.ProcessFailed(process, ex);
                }
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 8,
                MaxMessagesPerTask = 1,
                BoundedCapacity = 16
            });

            fileNameStep.LinkTo(uploadStep, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            pipelineStart = fileNameStep;
            pipelineEnd = uploadStep;
        }

        public async Task UploadAsync(IEnumerable<AssetModel> assets)
        {
            foreach (var asset in assets)
            {
                await pipelineStart.SendAsync(asset);
            }
        }

        public Task CompleteAsync()
        {
            pipelineEnd.Complete();

            return pipelineEnd.Completion;
        }
    }
}
