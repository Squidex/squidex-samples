// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using CommandDotNet;
using ConsoleTables;
using FluentValidation;
using FluentValidation.Attributes;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [Command(Name = "apps", Description = "Manages apps.")]
        [SubCommand]
        public sealed class Apps
        {
            private readonly IConfigurationService configuration;
            private readonly ILogger log;

            public Apps(IConfigurationService configuration, ILogger log)
            {
                this.configuration = configuration;

                this.log = log;
            }

            [Command(Name = "list", Description = "List all schemas.")]
            public async Task List(ListArguments arguments)
            {
                var session = configuration.StartSession();

                var apps = await session.Apps.GetAppsAsync();

                if (arguments.Table)
                {
                    var table = new ConsoleTable("Id", "Name", "LastUpdate");

                    foreach (var app in apps)
                    {
                        table.AddRow(app.Id, app.Name, app.LastModified);
                    }

                    table.Write(Format.Default);
                }
                else
                {
                    log.WriteLine(apps.JsonPrettyString());
                }
            }

            [Command(Name = "create", Description = "Creates a squidex app.")]
            public async Task Create(CreateArguments arguments)
            {
                var session = configuration.StartSession();

                var name = arguments.Name;

                if (string.IsNullOrWhiteSpace(name))
                {
                    name = session.App;
                }

                var request = new CreateAppDto
                {
                    Name = name
                };

                await session.Apps.PostAppAsync(request);

                log.WriteLine("> App created.");
            }

            [Validator(typeof(Validator))]
            public sealed class ListArguments : IArgumentModel
            {
                [Option(LongName = "table", ShortName = "t", Description = "Output as table")]
                public bool Table { get; set; }

                public sealed class Validator : AbstractValidator<ListArguments>
                {
                }
            }

            [Validator(typeof(Validator))]
            public sealed class CreateArguments : IArgumentModel
            {
                [Operand(Name = "name", Description = "The name of the app. If not provided then app configured in currentApp gets created.")]
                public string Name { get; set; }

                public sealed class Validator : AbstractValidator<ListArguments>
                {
                }
            }
        }
    }
}
