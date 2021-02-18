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
                    var filePath = Path.Combine(directoryInfo.FullName, $"assets/files/{asset.Id}.blob");
                    var fileHash = GetFileHash(filePath);

                    if (fileHash == null || !string.Equals(asset.FileHash, fileHash))
                    {
                        var response = await session.Assets.GetAssetContentBySlugAsync(session.App, asset.Id, string.Empty);

                        await using (response.Stream)
                        {
                            await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
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

            var assets = new List<AssetModel>();
            var assetBatch = 0;

            Task SaveAsync()
            {
                var model = new AssetsModel
                {
                    Assets = assets
                };

                return log.DoSafeAsync($"Exporting Assets ({assetBatch})", async () =>
                {
                    await jsonHelper.WriteWithSchema(directoryInfo, $"assets/{assetBatch}.json", model, "../__json/assets");
                });
            }

            var paths = new Dictionary<string, string>();

            await session.Assets.GetAllAsync(session.App, async asset =>
            {
                var path = string.Empty;

                if (asset.ParentId != null)
                {
                    if (!paths.TryGetValue(asset.ParentId, out path))
                    {
                        var folders = await session.Assets.GetAssetFoldersAsync(session.App, asset.ParentId);

                        foreach (var folder in folders.Items)
                        {
                            var names = folders.Path.Select(x => x.FolderName).Union(Enumerable.Repeat(folder.FolderName, 1));

                            paths[folder.Id] = string.Join("/", names);
                        }

                        for (var i = 0; i < folders.Path.Count; i++)
                        {
                            var names = folders.Path.Take(i + 1).Select(x => x.FolderName);

                            paths[folders.Path.ElementAt(i).Id] = string.Join("/", names);
                        }
                    }

                    if (!paths.TryGetValue(asset.ParentId, out path))
                    {
                        path = string.Empty;

                        paths[asset.ParentId] = path;
                    }
                }

                assets.Add(new AssetModel
                {
                    Id = asset.Id,
                    Metadata = asset.Metadata,
                    MimeType = asset.MimeType,
                    Slug = asset.Slug,
                    FileName = asset.FileName,
                    FileHash = asset.FileHash,
                    Path = path,
                    Tags = asset.Tags,
                    IsProtected = asset.IsProtected
                });

                if (assets.Count > 50)
                {
                    await SaveAsync();

                    assets.Clear();
                    assetBatch++;
                }

                if (assets.Count > 0)
                {
                    await SaveAsync();
                }

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
