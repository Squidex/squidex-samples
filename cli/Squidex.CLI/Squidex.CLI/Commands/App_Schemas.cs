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

                var schema = sourceSchema.Name;

                if (string.IsNullOrWhiteSpace(schema))
                {
                    throw new SquidexException("Schema name cannot be empty.");
                }

                try
                {
                    targetSchema = await schemasClient.GetSchemaAsync(app, schema);
                }
                catch
                {
                    targetSchema = null;
                }

                if (targetSchema == null)
                {
                    var request = JsonConvert.DeserializeObject<CreateSchemaDto>(schemaText);

                    request.Publish = sourceSchema.IsPublished;
                    request.Singleton = sourceSchema.IsSingleton;

                    await schemasClient.PostSchemaAsync(app, request);

                    Console.WriteLine("> Created schema because it does not exists in the target system.");
                }
                else
                {
                    var sourceFieldNames = sourceSchema.Fields.OrEmpty().Select(x => x.Name).ToList();
                    var targetFieldNames = targetSchema.Fields.OrEmpty().Select(x => x.Name).ToList();

                    var sourceMapping = sourceSchema.Fields.OrEmpty().ToDictionary(x => x.Name, x => x.FieldId);

                    if (!arguments.NoFieldDeletion)
                    {
                        foreach (var targetField in targetSchema.Fields.OrEmpty())
                        {
                            var sourceField = sourceSchema.Fields.OrEmpty().FirstOrDefault(x => x.Name == targetField.Name);

                            if (sourceField == null)
                            {
                                await schemasClient.DeleteFieldAsync(app, schema, targetField.FieldId);

                                Console.WriteLine($" - Field {targetField.Name} deleted.");

                                targetFieldNames.Remove(targetField.Name);
                            }
                        }
                    }

                    foreach (var sourceField in sourceSchema.Fields?.OrEmpty())
                    {
                        var targetField = targetSchema.Fields.OrEmpty().FirstOrDefault(x => x.Name == sourceField.Name);

                        var fieldId = 0L;

                        if (targetField == null)
                        {
                            fieldId = await schemasClient.CreateFieldAsync(app, schema, sourceField);

                            sourceMapping[sourceField.Name] = fieldId;

                            Console.WriteLine($" - Field {targetField.Name} created.");
                        }
                        else if (targetField.Partitioning == sourceField.Partitioning && targetField.Properties.GetType() == sourceField.Properties.GetType())
                        {
                            if (!sourceField.Properties.JsonEquals(targetField.Properties))
                            {
                                var request = new UpdateFieldDto
                                {
                                    Properties = sourceField.Properties
                                };

                                fieldId = targetField.FieldId;

                                await schemasClient.PutFieldAsync(app, schema, fieldId, request);

                                Console.WriteLine($" - Field {targetField.Name} updated.");
                            }
                        }
                        else if (!arguments.NoFieldRecreation)
                        {
                            await schemasClient.DeleteFieldAsync(app, schema, targetField.FieldId);

                            fieldId = await schemasClient.CreateFieldAsync(app, schema, sourceField);

                            sourceMapping[sourceField.Name] = fieldId;

                            Console.WriteLine($" - Field {targetField.Name} recreated.");
                        }

                        if (!sourceField.IsHidden.BoolEquals(targetField?.IsHidden))
                        {
                            await schemasClient.HideFieldAsync(app, schema, fieldId);

                            Console.WriteLine($" - Field {targetField.Name} hidden.");
                        }

                        if (!sourceField.IsLocked.BoolEquals(targetField?.IsLocked))
                        {
                            await schemasClient.LockFieldAsync(app, schema, fieldId);

                            Console.WriteLine($" - Field {targetField.Name} locked.");
                        }

                        if (!sourceField.IsDisabled.BoolEquals(targetField?.IsDisabled))
                        {
                            await schemasClient.DisableFieldAsync(app, schema, fieldId);

                            Console.WriteLine($" - Field {targetField.Name} disabled.");
                        }
                    }

                    if (!sourceSchema.Properties.JsonEqualsOrNew(targetSchema.Properties))
                    {
                        var request = new UpdateSchemaDto
                        {
                            Hints = targetSchema?.Properties?.Hints,
                            Label = targetSchema?.Properties?.Label,
                        };

                        await schemasClient.PutSchemaAsync(app, schema, request);

                        Console.WriteLine(" - Schema properties updated");
                    }

                    if (!sourceSchema.IsPublished.BoolEquals(targetSchema.IsPublished))
                    {
                        if (targetSchema.IsPublished == true)
                        {
                            await schemasClient.PublishSchemaAsync(app, schema);

                            Console.WriteLine($" - Schema published");
                        }
                        else
                        {
                            await schemasClient.UnpublishSchemaAsync(app, schema);

                            Console.WriteLine($" - Schema unpublished");
                        }
                    }

                    if (targetFieldNames.Count > 0 && targetFieldNames.Count == sourceFieldNames.Count && !targetFieldNames.JsonEqualsOrNew(sourceFieldNames))
                    {
                        var request = new ReorderFieldsDto
                        {
                            FieldIds = sourceFieldNames.Select(x => sourceMapping[x]).ToList()
                        };

                        // await schemasClient.O(app, schema);

                        Console.WriteLine($" - Schema fields reordered");
                    }
                }

                if (!sourceSchema.Category.StringEquals(targetSchema?.Category))
                {
                    var request = new ChangeCategoryDto
                    {
                        Name = sourceSchema.Category
                    };

                    await schemasClient.PutCategoryAsync(app, schema, request);

                    Console.WriteLine($" - Schema moved to category {sourceSchema.Category}");
                }

                if (!sourceSchema.PreviewUrls.JsonEquals(targetSchema?.PreviewUrls))
                {
                    // await schemasClient.O(app, schema);

                    Console.WriteLine($" - Schema preview urls configured");
                }

                if (!sourceSchema.ScriptChange.StringEquals(targetSchema?.ScriptChange) ||
                    !sourceSchema.ScriptCreate.StringEquals(targetSchema.ScriptCreate) ||
                    !sourceSchema.ScriptDelete.StringEquals(targetSchema.ScriptDelete) ||
                    !sourceSchema.ScriptQuery.StringEquals(targetSchema.ScriptQuery) ||
                    !sourceSchema.ScriptUpdate.StringEquals(targetSchema.ScriptUpdate))
                {
                    var request = new ConfigureScriptsDto
                    {
                        ScriptChange = sourceSchema.ScriptChange,
                        ScriptCreate = sourceSchema.ScriptCreate,
                        ScriptDelete = sourceSchema.ScriptDelete,
                        ScriptQuery = sourceSchema.ScriptQuery,
                        ScriptUpdate = sourceSchema.ScriptUpdate,
                    };

                    await schemasClient.PutSchemaScriptsAsync(app, schema, request);

                    Console.WriteLine(" - Schema scripts configured.");
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
