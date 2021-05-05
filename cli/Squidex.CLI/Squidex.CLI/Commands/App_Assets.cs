// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO;
using System.Threading.Tasks;
using CommandDotNet;
using FluentValidation;
using FluentValidation.Attributes;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.Sync.Assets;
using Squidex.CLI.Configuration;

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
            public async Task List(ImportArguments arguments)
            {
                var session = configuration.StartSession();

                var assets = session.Assets;

                var folder = new DirectoryInfo(arguments.Folder);
                var folderTree = new FolderTree(session);

                foreach (var file in folder.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    var relativeFolder = Path.GetRelativePath(folder.FullName, file.Directory.FullName);
                    var relativePath = Path.GetRelativePath(folder.FullName, file.FullName);

                    var targetFolder = arguments.TargetFolder;

                    if (!string.IsNullOrWhiteSpace(relativePath) && relativePath != ".")
                    {
                        targetFolder = Path.Combine(targetFolder, relativeFolder);
                    }

                    var parentId = await folderTree.GetIdAsync(targetFolder);

                    await log.DoSafeLineAsync($"Uploading {relativePath}", async () =>
                    {
                        await assets.PostAssetAsync(session.App, parentId, duplicate: arguments.Duplicate, file: file);
                    });
                }

                log.WriteLine("> Import completed");
            }

            [Validator(typeof(Validator))]
            public sealed class ImportArguments : IArgumentModel
            {
                [Operand(Name = "folder", Description = "The source folder.")]
                public string Folder { get; set; }

                [Option(ShortName = "t", LongName = "target", Description = "Path to the target folder.")]
                public string TargetFolder { get; set; }

                [Option(ShortName = "d", LongName = "duplicate", Description = "Duplicate the asset.")]
                public bool Duplicate { get; set; }

                public sealed class Validator : AbstractValidator<ImportArguments>
                {
                    public Validator()
                    {
                        RuleFor(x => x.Folder).NotEmpty();
                    }
                }
            }
        }
    }
}
