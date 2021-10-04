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
        private readonly ActionBlock<AssetModel> pipeline;

        public UploadPipeline(ISession session, ILogger log, IFileSystem fs)
        {
            var tree = new FolderTree(session);

            pipeline = new ActionBlock<AssetModel>(async asset =>
            {
                var process = $"Uploading {asset.Id}";

                try
                {
                    var assetFile = fs.GetBlobFile(asset.Id);

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
        }

        public async Task UploadAsync(IEnumerable<AssetModel> assets)
        {
            foreach (var asset in assets)
            {
                await pipeline.SendAsync(asset);
            }
        }

        public Task CompleteAsync()
        {
            pipeline.Complete();

            return pipeline.Completion;
        }
    }
}
