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
    public sealed class AssetsSynchronizer : ISynchronizer
    {
        private const string Ref = "../__json/asset";

        private readonly ILogger log;

        public int Order => -1000;

        public string Name => "Assets";

        public AssetsSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public async Task ExportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var pipeline = new ActionBlock<AssetDto>(async asset =>
            {
                var process = $"Downloading {asset.Id}";

                try
                {
                    await jsonHelper.WriteWithSchemaAs<AssetModel>(directoryInfo, $"assets/{asset.Id}.json", asset, Ref);

                    var filePath = Path.Combine(directoryInfo.FullName, $"assets/files/{asset.Id}.blob");
                    var fileHash = GetFileHash(filePath);

                    if (fileHash == null || !string.Equals(asset.FileHash, fileHash))
                    {
                        var response = await session.Assets.GetAssetContentBySlugAsync(session.App, asset.Id, string.Empty);

                        using (response.Stream)
                        {
                            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
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
            });

            await session.Assets.GetAllAsync(session.App, async asset =>
            {
                await pipeline.SendAsync(asset);
            });

            pipeline.Complete();

            await pipeline.Completion;
        }

        private static string GetFileHash(string path)
        {
            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

                    var buffer = new byte[4096];
                    var bytesRead = 0;

                    while ((bytesRead = fileStream.Read(buffer)) > 0)
                    {
                        incrementalHash.AppendData(buffer, 0, bytesRead);
                    }

                    return Convert.ToBase64String(incrementalHash.GetHashAndReset());
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            throw new NotImplementedException();
        }

        public async Task GenerateSchemaAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<AssetMeta>(directoryInfo, "assets.json");

            var sample = new AssetModel { FileName = "my.file.txt", MimeType = "plain/text" };

            await jsonHelper.WriteWithSchema(directoryInfo, "assets/__asset.json", sample, Ref);
        }
    }
}
