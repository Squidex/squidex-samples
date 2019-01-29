// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using CommandDotNet;
using CommandDotNet.Attributes;
using ConsoleTables;
using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [ApplicationMetadata(Name = "schemas", Description = "Manage schemas.")]
        [SubCommand]
        public class Schemas
        {
            [InjectProperty]
            public IConfigurationService Configuration { get; set; }

            [ApplicationMetadata(Name = "list", Description = "List all schemas.")]
            public async Task List(ListArguments arguments)
            {
                var (app, service) = Configuration.GetClient();

                var schemasClient = service.CreateSchemasClient();
                var schemas = await schemasClient.GetSchemasAsync(app);

                if (arguments.Table)
                {
                    var table = new ConsoleTable("Id", "Name", "Published", "LastUpdate");

                    foreach (var schema in schemas)
                    {
                        table.AddRow(schema.Id, schema.Name, schema.IsPublished, schema.LastModified);
                    }

                    table.Write(Format.Default);
                }
                else
                {
                    Console.WriteLine(schemas.JsonPrettyString());
                }
            }

            [ApplicationMetadata(Name = "get", Description = "Get a schema by name.")]
            public async Task Get(GetArguments arguments)
            {
                var (app, service) = Configuration.GetClient();

                var schemasClient = service.CreateSchemasClient();
                var schema = await schemasClient.GetSchemaAsync(app, arguments.Name);

                Console.WriteLine(schema.JsonPrettyString());
            }

            [ApplicationMetadata(Name = "sync", Description = "Sync the schema.")]
            public async Task Sync(SyncArguments arguments)
            {
                var (app, service) = Configuration.GetClient();

                var schemasClient = service.CreateSchemasClient();

                var schemaText = (string)null;
                var schemaName = arguments.Name;

                var targetSchema = (SchemaDetailsDto)null;
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
                        var sourceSchema = JsonConvert.DeserializeObject<SchemaDetailsDto>(schemaText);

                        schemaName = sourceSchema.Name;
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

                try
                {
                    targetSchema = await schemasClient.GetSchemaAsync(app, schemaName);
                }
                catch
                {
                    targetSchema = null;
                }

                if (targetSchema == null)
                {
                    var create = JsonConvert.DeserializeObject<CreateSchemaDto>(schemaText);

                    create.Name = schemaName;

                    await schemasClient.PostSchemaAsync(app, create);

                    Console.WriteLine("> Created schema because it does not exists in the target system.");
                }
                else
                {
                    var request = JsonConvert.DeserializeObject<SynchronizeSchemaDto>(schemaText);

                    request.NoFieldDeletion = arguments.NoFieldDeletion;
                    request.NoFieldRecreation = arguments.NoFieldRecreation;

                    await schemasClient.PutSchemaSyncAsync(app, schemaName, request);

                    Console.WriteLine("> Synchronized schema");
                }
            }

            [Validator(typeof(Validator))]
            public sealed class ListArguments : IArgumentModel
            {
                [Option(Description = "Output as table")]
                public bool Table { get; set; }

                public sealed class Validator : AbstractValidator<ListArguments>
                {
                }
            }

            [Validator(typeof(GetArgumentsValidator))]
            public sealed class GetArguments : IArgumentModel
            {
                [Argument(Name = "name", Description = "The name of the schema.")]
                public string Name { get; set; }

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
                [Argument(Name = "file", Description = "The file with the schema json.")]
                public string File { get; set; }

                [Option(LongName = "name", Description = "The new schema name.")]
                public string Name { get; set; }

                [Option(LongName = "no-delete", Description = "Do not delete fields.")]
                public bool NoFieldDeletion { get; set; }

                [Option(LongName = "no-recreate", Description = "Do not recreate fields.")]
                public bool NoFieldRecreation { get; set; }

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
