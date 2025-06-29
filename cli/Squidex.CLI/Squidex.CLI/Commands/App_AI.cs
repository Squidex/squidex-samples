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
using Squidex.CLI.Configuration;

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.CLI.Commands;

public partial class App
{
    [Command("ai", Description = "Uses AI commands.")]
    [Subcommand]
    public sealed class AI(IConfigurationService configuration, IConfigurationStore configurationStore, ILogger log)
    {
        [Command("generate-contents", Description = "Generates content items and the corresponding schema.",
            ExtendedHelpText =
@"Use descriptions with the following syntax:
    - ""The ten biggest countries with name, iso2code and area""
")]
        public async Task GenerateContents(GenerateArguments arguments)
        {
            var request = arguments.ToRequest();
            var queryCache = new ConfigurationQueryCache(configurationStore);
            var generator = new AIContentGenerator(queryCache);
            var generated = await generator.GenerateAsync(request);

            if (!arguments.Execute)
            {
                log.WriteLine($"Schema Name: {generated.Schema.Name}");
                log.WriteLine();
                log.WriteLine("Schema Fields:");

                var schemaTable = new ConsoleTable("Name", "Type", "Required", "Localized");

                foreach (var field in generated.Schema.Fields)
                {
                    schemaTable.AddRow(field.Name, field.Type, field.IsRequired, field.IsLocalized);
                }

                log.WriteLine(schemaTable.ToString());
                log.WriteLine();
                log.WriteLine("Contents:");

                foreach (var content in generated.Contents)
                {
                    log.WriteJson(content);
                    log.WriteLine();
                }

                if (generated.Contents.Count > 30)
                {
                    log.WriteLine();
                    log.WriteLine($"Total contents: {generated.Contents.Count}");
                }
            }
            else
            {
                var executor = new AIContentExecutor(configuration.StartSession(arguments.App), log);

                await executor.ExecuteAsync(request, generated, default);
            }
        }

        public sealed class GenerateArguments : AppArguments
        {
            [Operand("description", Description = "The description of your content.")]
            public string Description { get; set; }

            [Option("schemaName", Description = "The optional schema name to override the result.")]
            public string SchemaName { get; set; }

            [Option('k', "key", Description = "The api key.")]
            public string OpenAIApiKey { get; set; }

            [Option("no-schema", Description = "Do not create the schema.")]
            public bool NoSchema { get; set; }

            [Option("no-contents", Description = "Do not create the schema.")]
            public bool NoContents { get; set; }

            [Option("images", Description = "Indicates if images should be generated.")]
            public bool GenerateImages { get; set; }

            [Option("delete-schema", Description = "Delete the previous schema, if it exists.")]
            public bool DeleteSchema { get; set; }

            [Option("model", Description = "The OpenAI chat model")]
            public string OpenAIChatModel { get; set; } = "gpt-4-turbo";

            [Option("image-model", Description = "The OpenAI image model")]
            public string OpenAIImageModel { get; set; } = "dall-e-2";

            [Option("num-contents", Description = "The number of content items to create.")]
            public int NumberOfContentItems { get; set; } = 3;

            [Option("num-attempts", Description = "The number of attempts. If an error happens the CLI will try to fix it with the AI.")]
            public int NumberOfAttempts { get; set; } = 3;

            [Option("language", Description = "The supported languages.")]
            public List<string> Languages { get; set; } = ["en"];

            [Option("execute", Description = "Execture the result and do not describe it.")]
            public bool Execute { get; set; }

            public sealed class Validator : AbstractValidator<GenerateArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Description).NotEmpty();
                    RuleFor(x => x.OpenAIApiKey).NotEmpty();
                }
            }

            public GenerateRequest ToRequest()
            {
                return new GenerateRequest
                {
                    Description = Description,
                    GenerateImages = GenerateImages,
                    NoContents = NoContents,
                    NoSchema = NoSchema,
                    NumberOfAttempts = NumberOfAttempts,
                    NumberOfContentItems = NumberOfContentItems,
                    Languages = [.. Languages],
                    OpenAIApiKey = OpenAIApiKey,
                    OpenAIChatModel = OpenAIChatModel,
                    OpenAIImageModel = OpenAIImageModel,
                    SchemaName = SchemaName,
                };
            }
        }
    }
}
