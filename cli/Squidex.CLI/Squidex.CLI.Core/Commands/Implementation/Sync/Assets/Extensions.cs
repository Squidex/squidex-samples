// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Security.Cryptography;
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.CLI.Commands.Implementation.Utils;
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

        public static string? GetFileHash(this IFile file, AssetDto asset)
        {
            return GetFileHash(file, asset.FileName);
        }

        public static string? GetFileHash(this IFile file)
        {
            return GetFileHash(file, file.Name);
        }

        public static string? GetFileHash(this IFile file, string fileName)
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

        public static BulkUpdateAssetsJobDto ToMove(this AssetModel model, string? parentId)
        {
            var bulkJob = model.ToJob(BulkUpdateAssetType.Move);

            bulkJob.ParentId = parentId;

            return bulkJob;
        }

        public static BulkUpdateAssetsJobDto ToAnnotate(this AssetModel model)
        {
            var bulkJob = model.ToJob(BulkUpdateAssetType.Annotate);

            SimpleMapper.Map(bulkJob, model);

            return bulkJob;
        }

        private static BulkUpdateAssetsJobDto ToJob(this AssetModel model, BulkUpdateAssetType type)
        {
            return new BulkUpdateAssetsJobDto { Id = model.Id, Type = type };
        }

        public static AssetModel ToModel(this AssetDto asset)
        {
            return SimpleMapper.Map(asset, new AssetModel());
        }
    }
}
