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
using System.Threading.Tasks;
using CommandDotNet;
using FluentValidation;
using HeyRed.Mime;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.CLI.Commands.Implementation.Sync.Assets;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [Command("assets", Description = "Manages assets.")]
        [Subcommand]
        public sealed class Assets
        {
            private readonly IConfigurationService configuration;
            private readonly ILogger log;

            public Assets(IConfigurationService configuration, ILogger log)
            {
                this.configuration = configuration;

                this.log = log;
            }

            [Command("import", Description = "Import all files from the source folder.")]
            public async Task Import(ImportArguments arguments)
            {
                var session = configuration.StartSession(arguments.App);

                var assets = session.Assets;

                using (var fs = await FileSystems.CreateAsync(arguments.Path, session.WorkingDirectory))
                {
                    var folderTree = new FolderTree(session);

                    var assetQuery = new AssetQuery();

                    foreach (var file in fs.GetFiles(FilePath.Root, ".*"))
                    {
                        var targetFolder = file.LocalFolderPath();

                        if (!string.IsNullOrWhiteSpace(arguments.TargetFolder))
                        {
                            targetFolder = Path.Combine(arguments.TargetFolder, targetFolder);
                        }

                        assetQuery.ParentId = await folderTree.GetIdAsync(targetFolder);
                        assetQuery.Filter = $"fileName eq '{file.Name}'";

                        var existings = await assets.GetAssetsAsync(session.App, assetQuery);
                        var existing = existings.Items.FirstOrDefault();

                        var fileHash = file.GetFileHash();

                        try
                        {
                            var fileParameter = new FileParameter(file.OpenRead(), file.Name, MimeTypesMap.GetMimeType(file.Name));

                            log.WriteLine($"Uploading: {file.FullName}");

                            if (existings.Items.Any(x => string.Equals(x.FileHash, fileHash, StringComparison.Ordinal)))
                            {
                                log.StepSkipped("Same hash.");
                            }
                            else if (existings.Items.Count > 1)
                            {
                                log.StepSkipped("Multiple candidates found.");
                            }
                            else if (existing != null)
                            {
                                await assets.PutAssetContentAsync(session.App, existing.Id, fileParameter);

                                log.StepSuccess("Existing Asset");
                            }
                            else
                            {
                                var result = await assets.PostAssetAsync(session.App, assetQuery.ParentId, null, arguments.Duplicate, fileParameter);

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

                    log.WriteLine("> Import completed");
                }
            }

            [Command("export", Description = "Export all files to the source folder.")]
            public async Task Export(ImportArguments arguments)
            {
                var session = configuration.StartSession(arguments.App);

                var assets = session.Assets;

                using (var fs = await FileSystems.CreateAsync(arguments.Path, session.WorkingDirectory))
                {
                    var folderTree = new FolderTree(session);
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
                        }
                    };

                    try
                    {
                        await assets.GetAllByQueryAsync(session.App, async asset =>
                        {
                            await downloadPipeline.DownloadAsync(asset);
                        },
                        new AssetQuery
                        {
                            ParentId = parentId
                        });
                    }
                    finally
                    {
                        await downloadPipeline.CompleteAsync();
                    }

                    log.WriteLine("> Export completed");
                }
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
}
