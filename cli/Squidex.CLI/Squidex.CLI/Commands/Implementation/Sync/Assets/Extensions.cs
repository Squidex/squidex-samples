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
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets
{
    public static class Extensions
    {
        public static IFile GetBlobFile(this IFileSystem fs, string id)
        {
            return fs.GetFile(GetBlobPath(id));
        }

        public static FilePath GetBlobPath(this string id)
        {
            return new FilePath("assets", "files", $"{id}.blob");
        }

        public static string GetFileHash(this IFile file, AssetDto asset)
        {
            return GetFileHash(file, asset.FileName);
        }

        public static string GetFileHash(this IFile file)
        {
            return GetFileHash(file, file.Name);
        }

        public static string GetFileHash(this IFile file, string fileName)
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

                    var hash = $"{fileHash}{fileName}{fileStream.Length}".Sha256Base64();

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
                ParentId = null,
                Permanent = false,
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
