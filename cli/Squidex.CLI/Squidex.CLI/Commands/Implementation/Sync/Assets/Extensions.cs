// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets
{
    public static class Extensions
    {
        public static IFile GetBlobFile(this IFileSystem fs, string id)
        {
            return fs.GetFile(new FilePath("assets", "files", $"{id}.blob"));
        }

        public static BulkUpdateAssetsJobDto ToMoveJob(this AssetModel model, string parentId)
        {
            return new BulkUpdateAssetsJobDto
            {
                Id = model.Id,
                Type = BulkUpdateAssetType.Move,
                ParentId = parentId
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
                Metadata = model.Metadata,
                Slug = model.Slug,
                Tags = model.Tags
            };
        }

        public static async Task<AssetModel> ToModelAsync(this AssetDto asset, FolderTree folders)
        {
            return new AssetModel
            {
                Id = asset.Id,
                Metadata = asset.Metadata,
                MimeType = asset.MimeType,
                Slug = asset.Slug,
                FileName = asset.FileName,
                FileHash = asset.FileHash,
                FolderPath = await folders.GetPathAsync(asset.ParentId),
                Tags = asset.Tags,
                IsProtected = asset.IsProtected
            };
        }
    }
}
