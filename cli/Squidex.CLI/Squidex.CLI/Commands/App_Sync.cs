﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using CommandDotNet;
using FluentValidation;
using FluentValidation.Attributes;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.Sync;
using Squidex.CLI.Configuration;

namespace Squidex.CLI.Commands
{
    public sealed partial class App
    {
        [Command(Name = "sync", Description = "Synchronizes apps.")]
        [SubCommand]
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

            [Command(Name = "template", Description = "Creates the sample folders.")]
            public async Task Template(TemplateArgument arguments)
            {
                await synchronizer.GenerateTemplateAsync(arguments.Folder);

                log.WriteLine("> Template generated.");
            }

            [Command(Name = "out", Description = "Exports the app to a folder")]
            public async Task Out(OutArguments arguments)
            {
                var session = configuration.StartSession();

                var options = new SyncOptions();

                await synchronizer.ExportAsync(arguments.Folder, options, session);

                log.WriteLine("> Synchronization completed.");
            }

            [Command(Name = "in", Description = "Imports the app from a folder")]
            public async Task In(InArguments arguments)
            {
                var session = configuration.StartSession();

                var options = new SyncOptions();

                await synchronizer.ImportAsync(arguments.Folder, options, session);

                log.WriteLine("> Synchronization completed.");
            }

            [Validator(typeof(Validator))]
            public sealed class TemplateArgument : IArgumentModel
            {
                [Operand(Name = "folder", Description = "The target folder to create the templates.")]
                public string Folder { get; set; }

                public sealed class Validator : AbstractValidator<TemplateArgument>
                {
                    public Validator()
                    {
                        RuleFor(x => x.Folder).NotEmpty();
                    }
                }
            }

            [Validator(typeof(Validator))]
            public sealed class InArguments : IArgumentModel
            {
                [Operand(Name = "folder", Description = "The target folder to synchronize.")]
                public string Folder { get; set; }

                public sealed class Validator : AbstractValidator<InArguments>
                {
                    public Validator()
                    {
                        RuleFor(x => x.Folder).NotEmpty();
                    }
                }
            }

            [Validator(typeof(Validator))]
            public sealed class OutArguments : IArgumentModel
            {
                [Operand(Name = "folder", Description = "The target folder to synchronize.")]
                public string Folder { get; set; }

                public sealed class Validator : AbstractValidator<InArguments>
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
