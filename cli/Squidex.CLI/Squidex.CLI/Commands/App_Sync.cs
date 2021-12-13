﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using CommandDotNet;
using FluentValidation;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.Sync;
using Squidex.CLI.Configuration;

namespace Squidex.CLI.Commands
{
    public sealed partial class App
    {
        [Command("sync", Description = "Synchronizes apps.")]
        [Subcommand]
        public sealed class Sync
        {
            private readonly IConfigurationService configuration;
            private readonly Synchronizer synchronizer;
            private readonly ILogger log;

            public Sync(IConfigurationService configuration, Synchronizer synchronizer, ILogger log)
            {
                this.configuration = configuration;
                this.synchronizer = synchronizer;

                this.log = log;
            }

            [Command("new", Description = "Creates a new folder with sample files how to create an app from json files.")]
            public async Task New(NewArgument arguments)
            {
                await synchronizer.GenerateTemplateAsync(arguments.Folder);

                log.WriteLine("> Folder generated.");
            }

            [Command("out", Description = "Exports the app to a folder")]
            public async Task Out(OutArguments arguments)
            {
                var session = configuration.StartSession(arguments.App);

                await synchronizer.ExportAsync(arguments.Folder, arguments.ToOptions(), session);

                log.WriteLine("> Synchronization completed.");
            }

            [Command("in", Description = "Imports the app from a folder")]
            public async Task In(InArguments arguments)
            {
                var session = configuration.StartSession(arguments.App, arguments.Emulate);

                await synchronizer.ImportAsync(arguments.Folder, arguments.ToOptions(), session);

                log.WriteLine("> Synchronization completed.");
            }

            [Command("targets", Description = "List all targets")]
            public void Targets()
            {
                foreach (var target in synchronizer.GetTargets())
                {
                    log.WriteLine("- {0}", target.ToLowerInvariant());
                }
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

                [Option('t', "targets", Description = "The targets to sync, e.g. schemas, workflows, app, rules.")]
                public string[] Targets { get; set; }

                [Option("language", Description = "The content language to synchronize.")]
                public string[] Languages { get; set; }

                [Option("delete", Description = "Use this flag to also delete entities.")]
                public bool Delete { get; set; }

                [Option("recreate", Description = "Use this flag to also recreate entities.")]
                public bool Recreate { get; set; }

                [Option("update-current-client", Description = "Also update the client that is used during the sync process.")]
                public bool UpdateCurrentClient { get; set; }

                [Option("emulate", Description = "Use this flag to not make any updates and to emulate the changes.")]
                public bool Emulate { get; set; }

                public SyncOptions ToOptions()
                {
                    return new SyncOptions
                    {
                        Delete = Delete,
                        Recreate = Recreate,
                        Languages = Languages,
                        Targets = Targets,
                        UpdateCurrentClient = UpdateCurrentClient
                    };
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

                [Option('t', "targets", Description = "The targets to sync, e.g. schemas, workflows, app, rules, contents.")]
                public string[] Targets { get; set; }

                public SyncOptions ToOptions()
                {
                    return new SyncOptions { Targets = Targets };
                }

                public sealed class Validator : AbstractValidator<OutArguments>
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
