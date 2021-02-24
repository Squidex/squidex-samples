// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets
{
    public sealed class DownloadPipeline
    {
        private readonly ActionBlock<AssetDto> pipeline;

        public DownloadPipeline(ISession session, ILogger log, DirectoryInfo directoryInfo)
        {
            pipeline = new ActionBlock<AssetDto>(async asset =>
            {
                var process = $"Downloading {asset.Id}";

                try
                {
                    var assetFile = directoryInfo.GetBlobFile(asset.Id);
                    var assetHash = GetFileHash(assetFile, asset);

                    Directory.CreateDirectory(assetFile.Directory.FullName);

                    if (assetHash == null || !string.Equals(asset.FileHash, assetHash))
                    {
                        var response = await session.Assets.GetAssetContentBySlugAsync(session.App, asset.Id, string.Empty);

                        await using (response.Stream)
                        {
                            await using (var fileStream = assetFile.OpenWrite())
                            {
                                await response.Stream.CopyToAsync(fileStream);
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
                MaxDegreeOfParallelism = 8,
                MaxMessagesPerTask = 1,
                BoundedCapacity = 16
            });
        }

        private static string GetFileHash(FileInfo fileInfo, AssetDto asset)
        {
            if (!fileInfo.Exists)
            {
                return null;
            }

            try
            {
                using (var fileStream = fileInfo.OpenRead())
                {
                    var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

                    var buffer = new byte[80000];
                    var bytesRead = 0;

                    while ((bytesRead = fileStream.Read(buffer)) > 0)
                    {
                        incrementalHash.AppendData(buffer, 0, bytesRead);
                    }

                    var fileHash = Convert.ToBase64String(incrementalHash.GetHashAndReset());

                    var hash = $"{fileHash}{asset.FileName}{asset.FileSize}".Sha256Base64();

                    return hash;
                }
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public Task DownloadAsync(AssetDto asset)
        {
            return pipeline.SendAsync(asset);
        }

        public Task CompleteAsync()
        {
            pipeline.Complete();

            return pipeline.Completion;
        }
    }
}
