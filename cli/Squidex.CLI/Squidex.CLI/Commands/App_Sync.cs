// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using CommandDotNet;
using ConsoleTables;
using FluentValidation;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.Sync;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.CLI.Configuration;

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.CLI.Commands;

public sealed partial class App
{
    [Command("sync", Description = "Synchronizes apps.")]
    [Subcommand]
    public sealed class Sync(IConfigurationService configuration, Synchronizer synchronizer, ILogger log)
    {
        [Command("new", Description = "Creates a new folder with sample files how to create an app from json files.")]
        public async Task New(NewArgument arguments)
        {
            var session = configuration.StartSession(arguments.App);

            await synchronizer.GenerateTemplateAsync(arguments.Folder, session);

            log.Completed("Folder generated.");
        }

        [Command("out", Description = "Exports the app to a folder.")]
        public async Task Out(OutArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            await synchronizer.ExportAsync(arguments.Folder, arguments.ToOptions(), session);

            if (arguments.Describe)
            {
                await synchronizer.Describe(arguments.Folder, session);
            }

            log.Completed("Export to folder completed.");
        }

        [Command("in", Description = "Imports the app from a folder.")]
        public async Task In(InArguments arguments)
        {
            var session = configuration.StartSession(arguments.App, arguments.Emulate);

            await synchronizer.ImportAsync(arguments.Folder, arguments.ToOptions(), session);

            log.Completed("Import from folder completed.");
        }

        [Command("describe", Description = "Describe the synced folder.")]
        public async Task Describe(DescribeArguments arguments)
        {
            var session = configuration.StartSession(arguments.App, false);

            await synchronizer.Describe(arguments.Folder, session);

            log.Completed("Describing completed.");
        }

        [Command("targets", Description = "List all targets.")]
        public void Targets()
        {
            var table = new ConsoleTable("Name", "Description");

            foreach (var (name, description) in synchronizer.GetTargets())
            {
                table.AddRow(name.ToLowerInvariant(), description);
            }

            log.WriteLine(table.ToString());
        }

        public sealed class NewArgument : AppArguments
        {
            [Operand("folder", Description = "The target folder to create the templates.")]
            public string Folder { get; set; }

            public sealed class Validator : AbstractValidator<NewArgument>
            {
                public Validator()
                {
                    RuleFor(x => x.Folder).NotEmpty();
                }
            }
        }

        public sealed class InArguments : AppArguments
        {
            [Operand("folder", Description = "The target folder to synchronize.")]
            public string Folder { get; set; }

            [Option('t', "targets", Description = "The targets to sync, e.g. ‘sync in -t contents -t schemas’. Use 'sync targets' to view all targets.")]
            public string[] Targets { get; set; }

            [Option("language", Description = "The content language to synchronize.")]
            public string[] Languages { get; set; }

            [Option("content-action", Description = "Defines how to handle content.")]
            public ContentAction ContentAction { get; set; }

            [Option("delete", Description = "Use this flag to also delete entities.")]
            public bool Delete { get; set; }

            [Option("patch-content", Description = "Make content updates as patch.")]
            public bool PatchContent { get; set; }

            [Option("recreate", Description = "Use this flag to also recreate entities.")]
            public bool Recreate { get; set; }

            [Option("skip-assets", Description = "Use this flag to sync asset folders but not assets.")]
            public bool SkipAssets { get; set; }

            [Option("update-current-client", Description = "Also update the client that is used during the sync process.")]
            public bool UpdateCurrentClient { get; set; }

            [Option("emulate", Description = "Use this flag to not make any updates and to emulate the changes.")]
            public bool Emulate { get; set; }

            public SyncOptions ToOptions()
            {
                var result = SimpleMapper.Map(this, new SyncOptions());

                if (PatchContent)
                {
                    result.ContentAction = ContentAction.UpsertPatch;
                }

                return result;
            }

            public sealed class Validator : AbstractValidator<InArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Folder).NotEmpty();
                }
            }
        }

        public sealed class OutArguments : AppArguments
        {
            [Operand("folder", Description = "The target folder to synchronize.")]
            public string Folder { get; set; }

            [Option('t', "targets", Description = "The targets to sync, e.g. ‘sync out -t contents -t schemas’. Use 'sync targets' to view all targets.")]
            public string[] Targets { get; set; }

            [Option("stream-contents", Description = "Use the new streaming API for contents.")]
            public bool StreamContents { get; set; }

            [Option("max-age", Description = "Content & assets created or last modified within time span defined.")]
            public TimeSpan? MaxAge { get; set; }

            [Option("describe", Description = "Create a README.md file.")]
            public bool Describe { get; set; }

            public SyncOptions ToOptions()
            {
                return new SyncOptions
                {
                    Targets = Targets,
                    MaxAgeDate = GetMaxAgeDate(),
                    StreamContents = StreamContents
                };
            }

            public sealed class Validator : AbstractValidator<OutArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Folder).NotEmpty();
                }
            }

            private DateTimeOffset GetMaxAgeDate()
            {
                return MaxAge == null ? DateTimeOffset.MinValue : new DateTimeOffset(DateTime.Today.Add(-(TimeSpan)MaxAge));
            }
        }

        public sealed class DescribeArguments : AppArguments
        {
            [Operand("folder", Description = "The target folder to describe.")]
            public string Folder { get; set; }

            public sealed class Validator : AbstractValidator<DescribeArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Folder).NotEmpty();
                }
            }
        }
    }
}
