// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets
{
    public sealed class UploadPipeline
    {
        private readonly ActionBlock<AssetModel> pipeline;

        public UploadPipeline(ISession session, ILogger log, DirectoryInfo directoryInfo)
        {
            pipeline = new ActionBlock<AssetModel>(async asset =>
            {
                var process = $"Uploading {asset.Id}";

                try
                {
                    var assetFile = directoryInfo.GetBlobFile(asset.Id);

                    using (var stream = assetFile.OpenRead())
                    {
                        var file = new FileParameter(stream, asset.FileName, asset.MimeType);

                        await session.Assets.PostUpsertAssetAsync(session.App, asset.Id, null, file);
                    }

                    log.ProcessCompleted(process);
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
