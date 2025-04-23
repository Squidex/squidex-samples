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
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.CLI.Commands;

public partial class App
{
    [Command("apps", Description = "Manages apps.")]
    [Subcommand]
    public sealed class Apps(IConfigurationService configuration, ILogger log)
    {
        [Command("list", Description = "List all apps.")]
        public async Task List(ListArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            var apps = await session.Client.Apps.GetAppsAsync();

            if (arguments.Table)
            {
                var table = new ConsoleTable("Id", "Name", "LastUpdate");

                foreach (var app in apps)
                {
                    table.AddRow(app.Id, app.Name, app.LastModified);
                }

                log.WriteLine(table.ToString());
            }
            else
            {
                log.WriteLine(apps.JsonPrettyString());
            }
        }

        [Command("create", Description = "Creates a squidex app.")]
        public async Task Create(CreateArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            var name = arguments.App;

            if (string.IsNullOrWhiteSpace(name))
            {
                name = session.App;
            }

            var request = new CreateAppDto
            {
                Name = name
            };

            await session.Client.Apps.PostAppAsync(request);

            log.Completed("App creation completed.");
        }

        [Command("delete", Description = "Delete the app.")]
        public async Task Delete(DeleteArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            var name = arguments.App;

            if (!string.Equals(session.App, arguments.Confirm, StringComparison.Ordinal))
            {
                throw new CLIException("Confirmed app name does not match with the session app name.");
            }

            await session.Client.Apps.DeleteAppAsync();

            log.Completed("App deletion completed.");
        }

        public sealed class ListArguments : AppArguments
        {
            [Option('t', "table", Description = "Output as table.")]
            public bool Table { get; set; }

            public sealed class Validator : AbstractValidator<ListArguments>
            {
            }
        }

        public sealed class CreateArguments : AppArguments
        {
            public sealed class Validator : AbstractValidator<CreateArguments>
            {
            }
        }

        public sealed class DeleteArguments : AppArguments
        {
            [Option("confirm", Description = "Confirm the name of the app.")]
            public string Confirm { get; set; }

            public sealed class Validator : AbstractValidator<DeleteArguments>
            {
            }
        }
    }
}
