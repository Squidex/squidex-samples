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

namespace Squidex.CLI.Commands.Implementation.Sync.Assets;

internal static class Extensions
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
        return file.GetFileHash(asset.FileName);
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
