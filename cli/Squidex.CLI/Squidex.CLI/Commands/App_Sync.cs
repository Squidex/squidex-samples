// ==========================================================================
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
            private readonly ILogger log;

            public Sync(IConfigurationService configuration, ILogger log)
            {
                this.configuration = configuration;

                this.log = log;
            }

            [Command(Name = "template", Description = "Creates the sample folders.")]
            public async Task Template(GetArguments arguments)
            {
                var templateGenerator = new TemplateGenerator(arguments.Folder);

                await templateGenerator.GenerateAsync();
            }

            [Command(Name = "full", Description = "Makes a full sync of a folder")]
            public async Task Full(FullArguments arguments)
            {
                var session = configuration.StartSession();

                var options = new SyncOptions();

                var templateGenerator = new Synchronizer(log, arguments.Folder, session, options);

                await templateGenerator.SyncAsync();
            }

            [Validator(typeof(TemplateArgumentsValidator))]
            public sealed class GetArguments : IArgumentModel
            {
                [Operand(Name = "folder", Description = "The target folder to create the templates.")]
                public string Folder { get; set; }

                public sealed class TemplateArgumentsValidator : AbstractValidator<GetArguments>
                {
                    public TemplateArgumentsValidator()
                    {
                        RuleFor(x => x.Folder).NotEmpty();
                    }
                }
            }

            [Validator(typeof(TemplateArgumentsValidator))]
            public sealed class FullArguments : IArgumentModel
            {
                [Operand(Name = "folder", Description = "The target folder to synchronize.")]
                public string Folder { get; set; }

                public sealed class TemplateArgumentsValidator : AbstractValidator<GetArguments>
                {
                    public TemplateArgumentsValidator()
                    {
                        RuleFor(x => x.Folder).NotEmpty();
                    }
                }
            }
        }
    }
}
