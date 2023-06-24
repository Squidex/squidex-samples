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
using Squidex.CLI.Commands.Implementation.AI;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;
using Squidex.Text;

#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable MA0048 // File name must match type name

namespace Squidex.CLI.Commands;

public partial class App
{
    [Command("ai", Description = "Uses AI commands.")]
    [Subcommand]
    public sealed class AI
    {
        private readonly IConfigurationService configuration;
        private readonly IConfigurationStore configurationStore;
        private readonly ILogger log;

        public AI(IConfigurationService configuration, IConfigurationStore configurationStore, ILogger log)
        {
            this.configuration = configuration;
            this.configurationStore = configurationStore;

            this.log = log;
        }

        [Command("generate-contents", Description = "Generates content items and the corresponding schema.",
            ExtendedHelpText =
@"Use descriptions with the following syntax:
    - ""The ten biggest countries with name, iso2code and area""
")]
        public async Task GenerateContents(GenerateArguments arguments)
        {
            var generator = new AIContentGenerator(configurationStore);
            var generated = await generator.GenerateAsync(arguments.Description, arguments.ApiKey, arguments.SchemaName);

            if (!arguments.Execute)
            {
                log.WriteLine($"Schema Name: {generated.SchemaName}");
                log.WriteLine();
                log.WriteLine("Schema Fields:");

                var schemaTable = new ConsoleTable("Name", "Type");

                foreach (var field in generated.SchemaFields)
                {
                    schemaTable.AddRow(field.Name, field.Properties.GetType().Name.WithoutPrefix(nameof(FieldPropertiesDto)).ToLowerInvariant());
                }

                log.WriteLine(schemaTable.ToString());
                log.WriteLine();
                log.WriteLine("Contents:");

                var contentsTable = new ConsoleTable(generated.SchemaFields.Select(x => x.Name).ToArray());

                foreach (var content in generated.Contents.Take(30))
                {
                    var values = generated.SchemaFields.Select(x => x.Name).Select(x => content.GetValueOrDefault(x));

                    contentsTable.AddRow(values.ToArray());
                }

                log.WriteLine(contentsTable.ToString());

                if (generated.Contents.Count > 30)
                {
                    log.WriteLine();
                    log.WriteLine($"Total contents: {generated.Contents.Count}");
                }
            }
            else
            {
                var session = configuration.StartSession(arguments.App);

                if (arguments.DeleteSchema)
                {
                    try
                    {
                        await session.Client.Schemas.DeleteSchemaAsync(generated.SchemaName);
                    }
                    catch (SquidexException ex)
                    {
                        if (ex.StatusCode != 400)
                        {
                            throw;
                        }
                    }
                }

                var schemaProcess = "Creating Schema";

                if (!arguments.NoSchema)
                {
                    await session.Client.Schemas.PostSchemaAsync(new CreateSchemaDto
                    {
                        Name = generated.SchemaName.Slugify(),
                        IsPublished = true,
                        IsSingleton = false,
                        Fields = generated.SchemaFields
                    });

                    log.ProcessCompleted(schemaProcess);
                }
                else
                {
                    log.ProcessSkipped(schemaProcess, "Disabled");
                }

                var contentsProcess = $"Creating {generated.Contents} content items";

                if (!arguments.NoContents)
                {
                    await session.Client.DynamicContents(generated.SchemaName).BulkUpdateAsync(new BulkUpdate
                    {
                        Jobs = generated.Contents.Select(x => new BulkUpdateJob
                        {
                            Data = x.ToDictionary(x => x.Key, x => new { iv = x.Value })
                        }).ToList()
                    });

                    log.ProcessCompleted(contentsProcess);
                }
                else
                {
                    log.ProcessSkipped(contentsProcess, "Disabled");
                }

                log.Completed("Generation completed.");
            }
        }

        public sealed class GenerateArguments : AppArguments
        {
            [Operand("description", Description = "The description of your content.")]
            public string Description { get; set; }

            [Option("schemaName", Description = "The optional schema name to override the result.")]
            public string SchemaName { get; set; }

            [Option('k', "key", Description = "The api key.")]
            public string ApiKey { get; set; }

            [Option("no-schema", Description = "Do not create the schema.")]
            public bool NoSchema { get; set; }

            [Option("no-contents", Description = "Do not create the schema.")]
            public bool NoContents { get; set; }

            [Option("delete-schema", Description = "Delete the previous schema, if it exists.")]
            public bool DeleteSchema { get; set; }

            [Option("execute", Description = "Execture the result and do not describe it.")]
            public bool Execute { get; set; }

            public sealed class Validator : AbstractValidator<GenerateArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Description).NotEmpty();
                    RuleFor(x => x.ApiKey).NotEmpty();
                }
            }
        }
    }
}
