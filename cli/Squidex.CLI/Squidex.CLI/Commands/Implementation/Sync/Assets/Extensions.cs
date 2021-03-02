// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets
{
    public static class Extensions
    {
        public static FileInfo GetBlobFile(this DirectoryInfo directoryInfo, string id)
        {
            return new FileInfo(Path.Combine(directoryInfo.FullName, $"assets/files/{id}.blob"));
        }

        public static BulkUpdateAssetsJobDto ToMoveJob(this AssetModel model)
        {
            return new BulkUpdateAssetsJobDto
            {
                Id = model.Id,
                Type = BulkUpdateAssetType.Move,
                ParentPath = model.FolderPath
            };
        }

        public static BulkUpdateAssetsJobDto ToAnnotateJob(this AssetModel model)
        {
            return new BulkUpdateAssetsJobDto
            {
                Id = model.Id,
                Type = BulkUpdateAssetType.Annotate,
                FileName = model.FileName,
                IsProtected = model.IsProtected,
                ParentId = null,
                ParentPath = null,
                Metadata = model.Metadata,
                Slug = model.Slug,
                Tags = model.Tags
            };
        }

        public static async Task<AssetModel> ToModelAsync(this AssetDto asset, ISession session, Dictionary<string, string> cache)
        {
            return new AssetModel
            {
                Id = asset.Id,
                Metadata = asset.Metadata,
                MimeType = asset.MimeType,
                Slug = asset.Slug,
                FileName = asset.FileName,
                FileHash = asset.FileHash,
                FolderPath = await asset.ResolvePathAsync(session, cache),
                Tags = asset.Tags,
                IsProtected = asset.IsProtected
            };
        }

        public static async Task<string> ResolvePathAsync(this AssetDto asset, ISession session, Dictionary<string, string> cache)
        {
            var path = string.Empty;

            if (asset.ParentId != null)
            {
                if (!cache.TryGetValue(asset.ParentId, out path))
                {
                    var folders = await session.Assets.GetAssetFoldersAsync(session.App, asset.ParentId);

                    foreach (var folder in folders.Items)
                    {
                        var names = folders.Path.Select(x => x.FolderName).Union(Enumerable.Repeat(folder.FolderName, 1));

                        cache[folder.Id] = string.Join("/", names);
                    }

                    for (var i = 0; i < folders.Path.Count; i++)
                    {
                        var names = folders.Path.Take(i + 1).Select(x => x.FolderName);

                        cache[folders.Path.ElementAt(i).Id] = string.Join("/", names);
                    }
                }

                if (!cache.TryGetValue(asset.ParentId, out path))
                {
                    path = string.Empty;

                    cache[asset.ParentId] = path;
                }
            }

            return path;
        }
    }
}
