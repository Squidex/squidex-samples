// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using CommandDotNet;
using FluentValidation;
using HeyRed.Mime;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.CLI.Commands.Implementation.Sync.Assets;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.CLI.Commands;

public partial class App
{
    [Command("assets", Description = "Manages assets.")]
    [Subcommand]
    public sealed class Assets(IConfigurationService configuration, ILogger log)
    {
        [Command("import", Description = "Import all files from the source folder.")]
        public async Task Import(ImportArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            using var fs = await FileSystems.CreateAsync(arguments.Path);
            var assetTree = new AssetFolderTree(session.Client.Assets);
            var assetQuery = new AssetQuery();

            foreach (var file in fs.GetFiles(FilePath.Root, ".*"))
            {
                var targetFolder = file.LocalFolderPath();

                if (!string.IsNullOrWhiteSpace(arguments.TargetFolder))
                {
                    targetFolder = Path.Combine(arguments.TargetFolder, targetFolder);
                }

                assetQuery.ParentId = await assetTree.GetIdAsync(targetFolder);
                assetQuery.Filter = $"fileName eq '{file.Name}'";

                var existings = await session.Client.Assets.GetAssetsAsync(assetQuery);
                var existing = existings.Items.FirstOrDefault();

                var fileHash = file.GetFileHash();

                try
                {
                    var fileParameter = new FileParameter(file.OpenRead(), file.Name, MimeTypesMap.GetMimeType(file.Name));

                    log.WriteLine($"Uploading: {file.FullName}");

                    if (existings.Items.Exists(x => string.Equals(x.FileHash, fileHash, StringComparison.Ordinal)))
                    {
                        log.StepSkipped("Same hash.");
                    }
                    else if (existings.Items.Count > 1)
                    {
                        log.StepSkipped("Multiple candidates found.");
                    }
                    else if (existing != null)
                    {
                        await session.Client.Assets.PutAssetContentAsync(existing.Id, fileParameter);

                        log.StepSuccess("Existing Asset");
                    }
                    else
                    {
                        var result = await session.Client.Assets.PostAssetAsync(assetQuery.ParentId, null, arguments.Duplicate, fileParameter);

                        if (result._meta?.IsDuplicate == "true")
                        {
                            log.StepSkipped("duplicate.");
                        }
                        else
                        {
                            log.StepSuccess("New Asset");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogExtensions.HandleException(ex, error => log.WriteLine("Error: {0}", error));
                }
                finally
                {
                    log.WriteLine();
                }
            }

            log.Completed("Import completed.");
        }

        [Command("export", Description = "Export all files to the source folder.")]
        public async Task Export(ImportArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            using var fs = await FileSystems.CreateAsync(arguments.Path);
            var folderTree = new AssetFolderTree(session.Client.Assets);
            var folderNames = new HashSet<string>();

            var parentId = await folderTree.GetIdAsync(arguments.TargetFolder);

            var downloadPipeline = new DownloadPipeline(session, log, fs)
            {
                FilePathProviderAsync = async asset =>
                {
                    var assetFolder = await folderTree.GetPathAsync(asset.ParentId);
                    var assetPath = asset.FileName;

                    if (!string.IsNullOrWhiteSpace(assetFolder))
                    {
                        assetPath = Path.Combine(assetFolder, assetPath);
                    }

                    if (!folderNames.Add(assetPath))
                    {
                        assetPath = Path.Combine(assetFolder!, $"{asset.Id}_{asset.FileName}");
                    }

                    return FilePath.Create(assetPath);
                },
            };

            try
            {
                await session.Client.Assets.GetAllByQueryAsync(
                    downloadPipeline.DownloadAsync,
                    new AssetQuery
                    {
                        ParentId = parentId,
                    });
            }
            finally
            {
                await downloadPipeline.CompleteAsync();
            }

            log.Completed("Export completed.");
        }

        public sealed class ImportArguments : AppArguments
        {
            [Operand("folder", Description = "The source folder.")]
            public string Path { get; set; }

            [Option('t', "target", Description = "Path to the target folder.")]
            public string TargetFolder { get; set; }

            [Option('d', "duplicate", Description = "Duplicate the asset.")]
            public bool Duplicate { get; set; }

            public sealed class Validator : AbstractValidator<ImportArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Path).NotEmpty();
                }
            }
        }

        public sealed class ExportArguments : AppArguments
        {
            [Operand("folder", Description = "The source folder.")]
            public string Path { get; set; }

            [Option('t', "target", Description = "Path to the target folder.")]
            public string SourceFolder { get; set; }

            public sealed class Validator : AbstractValidator<ExportArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Path).NotEmpty();
                }
            }
        }
    }
}
