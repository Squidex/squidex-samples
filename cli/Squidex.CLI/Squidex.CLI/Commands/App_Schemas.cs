// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using CommandDotNet;
using ConsoleTables;
using FluentValidation;
using Newtonsoft.Json;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Models;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;

#pragma warning disable MA0048 // File name must match type name
#pragma warning disable IDE0059 // Value assigned to symbol is never used

namespace Squidex.CLI.Commands;

public partial class App
{
    [Command("schemas", Description = "Manage schemas.")]
    [Subcommand]
    public sealed class Schemas(IConfigurationService configuration, ILogger log)
    {
        [Command("list", Description = "List all schemas.")]
        public async Task List(ListArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            var schemas = await session.Client.Schemas.GetSchemasAsync();

            if (arguments.Table)
            {
                var table = new ConsoleTable("Id", "Name", "Published", "LastUpdate");

                foreach (var schema in schemas.Items)
                {
                    table.AddRow(schema.Id, schema.Name, schema.IsPublished, schema.LastModified);
                }

                log.WriteLine(table.ToString());
            }
            else
            {
                log.WriteLine(schemas.JsonPrettyString());
            }
        }

        [Command("get", Description = "Get a schema by name.")]
        public async Task Get(GetArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            var schema = await session.Client.Schemas.GetSchemaAsync(arguments.Schema);

            if (arguments.WithReferencedNames)
            {
                var allSchemas = await session.Client.Schemas.GetSchemasAsync();

                var result = new SchemaWithRefs<SchemaDto>(schema).EnrichSchemaNames(allSchemas.Items);

                log.WriteLine(result.JsonPrettyString());
            }
            else
            {
                log.WriteLine(schema.JsonPrettyString());
            }
        }

        [Command("sync", Description = "Sync the schema.")]
        public async Task Sync(SyncArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            var schemaText = string.Empty;
            var schemaName = arguments.Schema;
            try
            {
                schemaText = await File.ReadAllTextAsync(arguments.File);
            }
            catch (IOException)
            {
                throw new CLIException("Cannot read schema file.");
            }

            if (string.IsNullOrWhiteSpace(schemaName))
            {
                try
                {
                    var sourceSchema = SchemaWithRefs<SchemaDto>.Parse(schemaText);

                    schemaName = sourceSchema.Schema.Name;
                }
                catch (JsonException ex)
                {
                    throw new CLIException($"Cannot deserialize schema: {ex.Message}");
                }
            }

            if (string.IsNullOrWhiteSpace(schemaName))
            {
                throw new CLIException("Schema name cannot be empty.");
            }

            SchemaDto? targetSchema;
            try
            {
                targetSchema = await session.Client.Schemas.GetSchemaAsync(schemaName);
            }
            catch
            {
                targetSchema = null;
            }

            if (targetSchema == null)
            {
                var request = SchemaWithRefs<CreateSchemaDto>.Parse(schemaText);

                if (!arguments.NoRefFix && request.ReferencedSchemas.Count != 0)
                {
                    var allSchemas = await session.Client.Schemas.GetSchemasAsync();

                    request.AdjustReferences(allSchemas.Items);
                }

                request.Schema.Name = schemaName;

                await session.Client.Schemas.PostSchemaAsync(request.Schema);

                log.Completed("Creation of schema completed.");
            }
            else
            {
                var request = SchemaWithRefs<SynchronizeSchemaDto>.Parse(schemaText);

                if (!arguments.NoRefFix && request.ReferencedSchemas.Count != 0)
                {
                    var allSchemas = await session.Client.Schemas.GetSchemasAsync();

                    request.AdjustReferences(allSchemas.Items);
                }

                request.Schema.NoFieldDeletion = arguments.NoFieldDeletion;
                request.Schema.NoFieldRecreation = arguments.NoFieldRecreation;

                await session.Client.Schemas.PutSchemaSyncAsync(schemaName, request.Schema);

                log.Completed("Synchronization of schema completed.");
            }
        }

        public sealed class ListArguments : AppArguments
        {
            [Option('t', "table", Description = "Output as table.")]
            public bool Table { get; set; }

            public sealed class Validator : AbstractValidator<ListArguments>
            {
            }
        }

        public sealed class GetArguments : AppArguments
        {
            [Operand("schema", Description = "The name of the schema.")]
            public string Schema { get; set; }

            [Option('r', "with-refs", Description = "Includes the names of the referenced schemas.")]
            public bool WithReferencedNames { get; set; }

            public sealed class Validator : AbstractValidator<GetArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Schema).NotEmpty();
                }
            }
        }

        public sealed class SyncArguments : AppArguments
        {
            [Operand("file", Description = "The file with the schema json.")]
            public string File { get; set; }

            [Option("schema", Description = "The new schema name.")]
            public string Schema { get; set; }

            [Option("no-delete", Description = "Do not delete fields.")]
            public bool NoFieldDeletion { get; set; }

            [Option("no-recreate", Description = "Do not recreate fields.")]
            public bool NoFieldRecreation { get; set; }

            [Option("no-ref-fix", Description = "Do not fix referenced.")]
            public bool NoRefFix { get; set; }

            public sealed class Validator : AbstractValidator<SyncArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.File).NotEmpty();
                }
            }
        }
    }
}
