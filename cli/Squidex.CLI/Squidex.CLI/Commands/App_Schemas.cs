// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet;
using ConsoleTables;
using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Models;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

#pragma warning disable IDE0059 // Value assigned to symbol is never used

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [Command(Name = "schemas", Description = "Manage schemas.")]
        [SubCommand]
        public sealed class Schemas
        {
            private readonly IConfigurationService configuration;
            private readonly ILogger log;

            public Schemas(IConfigurationService configuration, ILogger log)
            {
                this.configuration = configuration;

                this.log = log;
            }

            [Command(Name = "list", Description = "List all schemas.")]
            public async Task List(ListArguments arguments)
            {
                var session = configuration.StartSession();

                var schemas = await session.Schemas.GetSchemasAsync(session.App);

                if (arguments.Table)
                {
                    var table = new ConsoleTable("Id", "Name", "Published", "LastUpdate");

                    foreach (var schema in schemas.Items)
                    {
                        table.AddRow(schema.Id, schema.Name, schema.IsPublished, schema.LastModified);
                    }

                    table.Write(Format.Default);
                }
                else
                {
                    log.WriteLine(schemas.JsonPrettyString());
                }
            }

            [Command(Name = "get", Description = "Get a schema by name.")]
            public async Task Get(GetArguments arguments)
            {
                var session = configuration.StartSession();

                var schema = await session.Schemas.GetSchemaAsync(session.App, arguments.Name);

                if (arguments.WithReferencedNames)
                {
                    var allSchemas = await session.Schemas.GetSchemasAsync(session.App);

                    var result = new SchemaWithRefs<SchemaDetailsDto>(schema).EnrichSchemaNames(allSchemas.Items);

                    log.WriteLine(result.JsonPrettyString());
                }
                else
                {
                    log.WriteLine(schema.JsonPrettyString());
                }
            }

            [Command(Name = "sync", Description = "Sync the schema.")]
            public async Task Sync(SyncArguments arguments)
            {
                var session = configuration.StartSession();

                var schemaText = string.Empty;
                var schemaName = arguments.Name;

                try
                {
                    schemaText = File.ReadAllText(arguments.File);
                }
                catch (IOException)
                {
                    throw new SquidexException("Cannot read schema file.");
                }

                if (string.IsNullOrWhiteSpace(schemaName))
                {
                    try
                    {
                        var sourceSchema = SchemaWithRefs<SchemaDetailsDto>.Parse(schemaText);

                        schemaName = sourceSchema.Schema.Name;
                    }
                    catch (JsonException ex)
                    {
                        throw new SquidexException($"Cannot deserialize schema: {ex.Message}");
                    }
                }

                if (string.IsNullOrWhiteSpace(schemaName))
                {
                    throw new SquidexException("Schema name cannot be empty.");
                }

                SchemaDetailsDto targetSchema;
                try
                {
                    targetSchema = await session.Schemas.GetSchemaAsync(session.App, schemaName);
                }
                catch
                {
                    targetSchema = null;
                }

                if (targetSchema == null)
                {
                    var request = SchemaWithRefs<CreateSchemaDto>.Parse(schemaText);

                    if (!arguments.NoRefFix && request.ReferencedSchemas.Any())
                    {
                        var allSchemas = await session.Schemas.GetSchemasAsync(session.App);

                        request.AdjustReferences(allSchemas.Items);
                    }

                    request.Schema.Name = schemaName;

                    await session.Schemas.PostSchemaAsync(session.App, request.Schema);

                    log.WriteLine("> Created schema because it does not exists in the target system.");
                }
                else
                {
                    var request = SchemaWithRefs<SynchronizeSchemaDto>.Parse(schemaText);

                    if (!arguments.NoRefFix && request.ReferencedSchemas.Any())
                    {
                        var allSchemas = await session.Schemas.GetSchemasAsync(session.App);

                        request.AdjustReferences(allSchemas.Items);
                    }

                    request.Schema.NoFieldDeletion = arguments.NoFieldDeletion;
                    request.Schema.NoFieldRecreation = arguments.NoFieldRecreation;

                    await session.Schemas.PutSchemaSyncAsync(session.App, schemaName, request.Schema);

                    log.WriteLine("> Synchronized schema");
                }
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

            [Validator(typeof(GetArgumentsValidator))]
            public sealed class GetArguments : IArgumentModel
            {
                [Operand(Name = "name", Description = "The name of the schema.")]
                public string Name { get; set; }

                [Option(LongName = "with-refs", ShortName = "r", Description = "Includes the names of the referenced schemas.")]
                public bool WithReferencedNames { get; set; }

                public sealed class GetArgumentsValidator : AbstractValidator<GetArguments>
                {
                    public GetArgumentsValidator()
                    {
                        RuleFor(x => x.Name).NotEmpty();
                    }
                }
            }

            [Validator(typeof(SyncArgumentsValidator))]
            public sealed class SyncArguments : IArgumentModel
            {
                [Operand(Name = "file", Description = "The file with the schema json.")]
                public string File { get; set; }

                [Option(LongName = "name", Description = "The new schema name.")]
                public string Name { get; set; }

                [Option(LongName = "no-delete", Description = "Do not delete fields.")]
                public bool NoFieldDeletion { get; set; }

                [Option(LongName = "no-recreate", Description = "Do not recreate fields.")]
                public bool NoFieldRecreation { get; set; }

                [Option(LongName = "no-ref-fix", Description = "Do not fix referenced.")]
                public bool NoRefFix { get; set; }

                public sealed class SyncArgumentsValidator : AbstractValidator<SyncArguments>
                {
                    public SyncArgumentsValidator()
                    {
                        RuleFor(x => x.File).NotEmpty();
                    }
                }
            }
        }
    }
}
