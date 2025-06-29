// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using CommandDotNet;
using FluentValidation;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.OpenLibrary;
using Squidex.CLI.Commands.Implementation.Sync;
using Squidex.CLI.Configuration;

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.CLI.Commands;

public sealed partial class App
{
    [Command("openlib", Description = "Openlibrary example.")]
    [Subcommand]
    public sealed class OpenLibrary(IConfigurationService configuration, Synchronizer synchronizer, ILogger log)
    {
        [Command("generate", Description = "Generate the necessary schemas.")]
        public async Task New(NewArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            await synchronizer.ImportAsync("assembly://Squidex.CLI.Commands.Implementation.OpenLibrary.Structure", new SyncOptions
            {
                Targets =
                [
                    "schemas",
                ],
                Recreate = true,
            }, session);

            log.Completed("Creation of schema completed.");
        }

        [Command("authors", Description = "Import the authors.")]
        public async Task Authors(ImportArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            await using (var stream = new FileStream(arguments.File, FileMode.Open))
            {
                var importer = new AuthorImporter(session, log);

                await importer.ImportAsync(stream);
            }

            log.Completed("Import of Authors completed.");
        }

        public sealed class NewArguments : AppArguments
        {
        }

        public sealed class ImportArguments : AppArguments
        {
            [Operand("file", Description = "The data dump file.")]
            public string File { get; set; }

            public sealed class Validator : AbstractValidator<ImportArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.File).NotEmpty();
                }
            }
        }
    }
}
