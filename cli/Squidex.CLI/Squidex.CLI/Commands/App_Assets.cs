﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet;
using FluentValidation;
using FluentValidation.Attributes;
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
        [Command(Name = "assets", Description = "Manages assets.")]
        [SubCommand]
        public sealed class Assets
        {
            private readonly IConfigurationService configuration;
            private readonly ILogger log;

            public Assets(IConfigurationService configuration, ILogger log)
            {
                this.configuration = configuration;

                this.log = log;
            }

            [Command(Name = "import", Description = "Import all files from the source folder.")]
            public async Task Import(ImportArguments arguments)
            {
                var session = configuration.StartSession();

                var assets = session.Assets;

                using (var fs = FileSystems.Create(arguments.Path))
                {
                    var folderTree = new FolderTree(session);

                    foreach (var file in fs.GetFiles(FilePath.Root, ".*"))
                    {
                        var targetFolder = file.LocalFolderPath();

                        if (!string.IsNullOrWhiteSpace(arguments.TargetFolder))
                        {
                            targetFolder = Path.Combine(arguments.TargetFolder, targetFolder);
                        }

                        var parentId = await folderTree.GetIdAsync(targetFolder);

                        var existings = await assets.GetAssetsAsync(session.App, new AssetQuery
                        {
                            ParentId = parentId,
                            Filter = $"fileName eq '{file.Name}'",
                            Top = 2
                        });

                        try
                        {
                            var fileParameter = new FileParameter(file.OpenRead(), file.Name, MimeTypesMap.GetMimeType(file.Name));

                            if (existings.Items.Count > 0)
                            {
                                var existing = existings.Items.First();

                                log.WriteLine($"Uploading: {file.FullName}");

                                await assets.PutAssetContentAsync(session.App, existing.Id, fileParameter);

                                log.StepSuccess();
                            }
                            else
                            {
                                log.WriteLine($"Uploading New: {file.FullName}");

                                var result = await assets.PostAssetAsync(session.App, parentId, duplicate: arguments.Duplicate, file: fileParameter);

                                if (result._meta?.IsDuplicate == "true")
                                {
                                    log.StepSkipped("duplicate.");
                                }
                                else
                                {
                                    log.StepSuccess();
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

            [Command(Name = "export", Description = "Export all files to the source folder.")]
            public async Task Export(ImportArguments arguments)
            {
                var session = configuration.StartSession();

                var assets = session.Assets;

                using (var fs = FileSystems.Create(arguments.Path))
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
                                assetPath = Path.Combine(assetFolder, $"{asset.Id}_{asset.FileName}");
                            }

                            return FilePath.Create(assetPath);
                        }
                    };

                    await assets.GetAllByQueryAsync(session.App, async asset =>
                    {
                        await downloadPipeline.DownloadAsync(asset);
                    },
                    new AssetQuery
                    {
                        ParentId = parentId
                    });

                    await downloadPipeline.CompleteAsync();

                    log.WriteLine("> Export completed");
                }
            }

            [Validator(typeof(Validator))]
            public sealed class ImportArguments : IArgumentModel
            {
                [Operand(Name = "folder", Description = "The source folder.")]
                public string Path { get; set; }

                [Option(ShortName = "t", LongName = "target", Description = "Path to the target folder.")]
                public string TargetFolder { get; set; }

                [Option(ShortName = "d", LongName = "duplicate", Description = "Duplicate the asset.")]
                public bool Duplicate { get; set; }

                public sealed class Validator : AbstractValidator<ImportArguments>
                {
                    public Validator()
                    {
                        RuleFor(x => x.Path).NotEmpty();
                    }
                }
            }

            [Validator(typeof(Validator))]
            public sealed class ExportArguments : IArgumentModel
            {
                [Operand(Name = "folder", Description = "The source folder.")]
                public string Path { get; set; }

                [Option(ShortName = "t", LongName = "target", Description = "Path to the target folder.")]
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