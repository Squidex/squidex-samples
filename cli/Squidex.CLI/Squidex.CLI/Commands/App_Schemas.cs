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
                var service = Configuration.GetClient();

                var schemasClient = service.CreateSchemasClient();
                var schemas = await schemasClient.GetSchemasAsync(service.App);

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
                var service = Configuration.GetClient();

                var schemasClient = service.CreateSchemasClient();
                var schema = await schemasClient.GetSchemaAsync(service.App, arguments.Name);

                Console.WriteLine(schema.JsonPrettyString());
            }

            [ApplicationMetadata(Name = "sync", Description = "Sync the schema.")]
            public async Task Sync(SyncArguments arguments)
            {
                var service = Configuration.GetClient();

                var schemasClient = service.CreateSchemasClient();

                var schemaText = (string)null;
                var sourceSchema = (SchemaDetailsDto)null;

                var targetSchema = (SchemaDetailsDto)null;
                try
                {
                    schemaText = File.ReadAllText(arguments.File);
                }
                catch (IOException)
                {
                    throw new SquidexException("Cannot read schema file.");
                }

                try
                {
                    sourceSchema = JsonConvert.DeserializeObject<SchemaDetailsDto>(schemaText);
                }
                catch (JsonException ex)
                {
                    throw new SquidexException($"Cannot deserialize schema: {ex.Message}");
                }

                if (string.IsNullOrWhiteSpace(sourceSchema.Name))
                {
                    throw new SquidexException("Schema name cannot be empty.");
                }

                try
                {
                    targetSchema = await schemasClient.GetSchemaAsync(service.App, sourceSchema.Name);
                }
                catch
                {
                    targetSchema = null;
                }

                if (targetSchema == null)
                {
                    var request = new CreateSchemaDto
                    {
                        Publish = sourceSchema.IsPublished,
                        Properties = sourceSchema.Properties,
                        Fields = sourceSchema.Fields.OrEmpty().Select(x => new CreateSchemaFieldDto
                        {
                            Name = x.Name,
                            IsDisabled = x.IsDisabled,
                            IsHidden = x.IsHidden,
                            IsLocked = x.IsLocked,
                            Nested = x.Nested.OrEmpty().Select(y => new CreateSchemaNestedFieldDto
                            {
                                Name = y.Name,
                                IsDisabled = y.IsDisabled,
                                IsHidden = y.IsHidden,
                                Properties = y.Properties,
                            }).ToList(),
                            Partitioning = x.Partitioning,
                            Properties = x.Properties,
                        }).ToList()
                    };

                    await schemasClient.PostSchemaAsync(service.App, request);

                    Console.WriteLine("> Created schema because it does not exists in the target system.");
                }
                else
                {
                    if (!arguments.NoFieldDeletion)
                    {
                        foreach (var targetField in targetSchema.Fields.OrEmpty())
                        {
                            var sourceField = sourceSchema.Fields.OrEmpty().FirstOrDefault(x => x.Name == targetField.Name);

                            if (sourceField == null)
                            {
                                await schemasClient.DeleteFieldAsync(service.App, sourceSchema.Name, targetField.FieldId);

                                Console.WriteLine($"> Field {targetField.Name} deleted.");
                            }
                            else
                            {
                                foreach (var targetNestedField in targetField.Nested)
                                {
                                    var sourceNestedField = sourceField.Nested.FirstOrDefault(x => x.Name == targetNestedField.Name);

                                    if (sourceNestedField == null)
                                    {
                                        await schemasClient.DeleteNestedFieldAsync(service.App, sourceSchema.Name, targetField.FieldId, targetNestedField.FieldId);

                                        Console.WriteLine($"> Field {targetField.Name}/{targetNestedField.Name} deleted.");
                                    }
                                }
                            }
                        }
                    }

                    foreach (var sourceField in sourceSchema.Fields?.OrEmpty())
                    {
                        var targetField = targetSchema.Fields.OrEmpty().FirstOrDefault(x => x.Name == sourceField.Name);

                        var id = 0L;

                        if (targetField == null || ((targetField.Partitioning != sourceField.Partitioning || targetField.Properties.GetType() != sourceField.Properties.GetType()) && !arguments.NoFieldRecreation))
                        {
                            var request = new AddFieldDto
                            {
                                Name = sourceField.Name,
                                Partitioning = sourceField.Partitioning,
                                Properties = sourceField.Properties
                            };

                            await schemasClient.PostFieldAsync(service.App, sourceSchema.Name, request);

                            id = targetField.FieldId;
                        }
                        else
                        {
                            var request = new UpdateFieldDto
                            {
                                Properties = sourceField.Properties
                            };

                            await schemasClient.PutFieldAsync(service.App, sourceSchema.Name, sourceField.FieldId, request);
                        }

                        if (sourceField.IsHidden != targetField?.IsHidden)
                        {
                            await schemasClient.HideFieldAsync(service.App, sourceField.Name, sourceField.FieldId);
                        }
                    }
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

            [Validator(typeof(GetArgumentsValidator))]
            public sealed class SyncArguments : IArgumentModel
            {
                [Argument(Name = "file", Description = "The file with the schema json.")]
                public string File { get; set; }

                [Option(LongName = "no-delete", Description = "Do not delete fields.")]
                public bool NoFieldDeletion { get; set; }

                [Option(LongName = "no-recreate", Description = "Do not recreate fields.")]
                public bool NoFieldRecreation { get; set; }

                public sealed class GetArgumentsValidator : AbstractValidator<GetArguments>
                {
                    public GetArgumentsValidator()
                    {
                        RuleFor(x => x.Name).NotEmpty();
                    }
                }
            }
        }
    }
}
