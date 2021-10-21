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
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets
{
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
                    var assetHash = GetFileHash(assetFile, asset);

                    if (assetHash == null || !string.Equals(asset.FileHash, assetHash))
                    {
                        var response = await session.Assets.GetAssetContentBySlugAsync(session.App, asset.Id, string.Empty);

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

            fileNameStep.LinkTo(downloadStep, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            pipelineStart = fileNameStep;
            pipelineEnd = downloadStep;
        }

        private static string GetFileHash(IFile file, AssetDto asset)
        {
            if (file == null)
            {
                return null;
            }

            try
            {
                using (var fileStream = file.OpenRead())
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
            return pipelineStart.SendAsync(asset);
        }

        public Task CompleteAsync()
        {
            pipelineEnd.Complete();

            return pipelineEnd.Completion;
        }
    }
}
