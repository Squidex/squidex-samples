// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CommandDotNet;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;

#pragma warning disable IDE0059 // Value assigned to symbol is never used

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [Command(Name = "content", Description = "Manage contents.")]
        [SubCommand]
        public sealed class Content
        {
            [InjectProperty]
            public IConfigurationService Configuration { get; set; }

            [Command(Name = "test-data", Description = "Generates test data.")]
            public async Task TestData(TestDataArguments arguments)
            {
                var (app, service) = Configuration.Setup();

                var taskForSchema = service.CreateSchemasClient().GetSchemaAsync(app, arguments.Schema);
                var taskForLanguages = service.CreateAppsClient().GetLanguagesAsync(app);

                await Task.WhenAll(
                    taskForSchema,
                    taskForLanguages);

                var datas = new List<DummyData>();

                if (arguments.Count > 0)
                {
                    var generator = new TestDataGenerator(taskForSchema.Result, taskForLanguages.Result);

                    for (var i = 0; i < arguments.Count; i++)
                    {
                        datas.Add(generator.GenerateTestData());
                    }
                }

                if (!string.IsNullOrWhiteSpace(arguments.File))
                {
                    File.AppendAllText(arguments.File, datas.JsonPrettyString());
                }
                else
                {
                    await ImportAsync(arguments, service, datas);
                }
            }

            [Command(Name = "import", Description = "Import the content to a schema.",
                ExtendedHelpText =
@"Use the following format to define fields from the CSV/JSON file:
    - name (for invariant fields)
    - name=other(for invariant fields from another field)
    - name.de=name (for localized fields)
")]
            public async Task Import(ImportArguments arguments)
            {
                var (_, service) = Configuration.Setup();

                if (arguments.Format == Format.JSON)
                {
                    var converter = new Json2SquidexConverter(arguments.Fields);

                    using (var stream = new FileStream(arguments.File, FileMode.Open, FileAccess.Read))
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            using (var reader = new JsonTextReader(streamReader))
                            {
                                var datas = converter.ReadAll(reader);

                                await ImportAsync(arguments, service, datas);
                            }
                        }
                    }
                }
                else
                {
                    var converter = new Csv2SquidexConverter(arguments.Fields);

                    using (var stream = new FileStream(arguments.File, FileMode.Open, FileAccess.Read))
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            var csvOptions = new CsvConfiguration(CultureInfo.InvariantCulture)
                            {
                                Delimiter = arguments.Delimiter
                            };

                            using (var reader = new CsvReader(streamReader, csvOptions))
                            {
                                var datas = converter.ReadAll(reader);

                                await ImportAsync(arguments, service, datas);
                            }
                        }
                    }
                }
            }

            [Command(Name = "export", Description = "Export the content for a schema.",
                ExtendedHelpText =
@"Use the following format to define fields for CSV:
    - id
    - createdBy
    - created
    - lastModifiedBy
    - lastModified
    - version
    - data.name (for invariant fields)
    - data.name.de (for localized fields)
    - name=data.name (for invariant fields with alias)
    - name (German)=data.name.de (for localized fields with alias)
")]
            public async Task Export(ExportArguments arguments)
            {
                var ctx = QueryContext.Default.Unpublished(arguments.Unpublished);

                var (_, service) = Configuration.Setup();

                var client = service.CreateContentsClient<DummyEntity, DummyData>(arguments.Schema);

                if (arguments.Format == Format.JSON)
                {
                    if (!string.IsNullOrWhiteSpace(arguments.Fields))
                    {
                        throw new SquidexException("Fields are not used for JSON export.");
                    }

                    var fileOrFolder = arguments.Output;

                    if (arguments.FilePerContent)
                    {
                        if (string.IsNullOrWhiteSpace(fileOrFolder))
                        {
                            fileOrFolder = $"{arguments.Schema}_{DateTime.UtcNow:yyyy-MM-dd-hh-mm-ss}";
                        }

                        if (!Directory.Exists(fileOrFolder))
                        {
                            Directory.CreateDirectory(fileOrFolder);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(fileOrFolder))
                        {
                            fileOrFolder = $"{arguments.Schema}_{DateTime.UtcNow:yyyy-MM-dd-hh-mm-ss}.json";
                        }

                        if (File.Exists(fileOrFolder))
                        {
                            File.Delete(fileOrFolder);
                        }
                    }

                    await ExportAsync(arguments, entity =>
                    {
                        if (arguments.FilePerContent)
                        {
                            File.WriteAllText(Path.Combine(fileOrFolder, $"{arguments.Schema}_{entity.Id}.json"), entity.JsonPrettyString());
                        }
                        else
                        {
                            File.AppendAllText(fileOrFolder, entity.JsonPrettyString());
                        }
                    });
                }
                else
                {
                    if (arguments.FilePerContent)
                    {
                        throw new SquidexException("Multiple files are not supported for CSV export.");
                    }

                    if (string.IsNullOrWhiteSpace(arguments.Fields))
                    {
                        throw new SquidexException("Fields must be defined for CSV export.");
                    }

                    var file = arguments.Output;

                    if (string.IsNullOrWhiteSpace(file))
                    {
                        file = $"{arguments.Schema}_{DateTime.UtcNow:yyyy-MM-dd-hh-mm-ss}.csv";
                    }

                    var converter = new Squidex2CsvConverter(arguments.Fields);

                    using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write))
                    {
                        using (var streamWriter = new StreamWriter(stream))
                        {
                            var csvOptions = new CsvConfiguration(CultureInfo.InvariantCulture)
                            {
                                Delimiter = ";"
                            };

                            using (var writer = new CsvWriter(streamWriter, csvOptions))
                            {
                                foreach (var fieldName in converter.FieldNames)
                                {
                                    writer.WriteField(fieldName);
                                }

                                writer.NextRecord();

                                await ExportAsync(arguments, entity =>
                                {
                                    foreach (var value in converter.GetValues(entity))
                                    {
                                        if (value is string text)
                                        {
                                            writer.WriteField(text, true);
                                        }
                                        else
                                        {
                                            writer.WriteField(value);
                                        }
                                    }

                                    writer.NextRecord();
                                });
                            }
                        }
                    }
                }
            }

            private async Task ImportAsync(IImortArgumentBase arguments, SquidexClientManager service, IEnumerable<DummyData> datas)
            {
                var client = service.CreateContentsClient<DummyEntity, DummyData>(arguments.Schema);

                var totalWritten = 0;

                var consoleTop = Console.CursorTop;

                var handled = new HashSet<string>();

                foreach (var data in datas)
                {
                    await client.CreateAsync(data, !arguments.Unpublished);

                    totalWritten++;

                    Console.WriteLine("> Imported: {0}.", totalWritten);
                    Console.SetCursorPosition(0, consoleTop);
                }

                Console.WriteLine("> Imported: {0}. Completed.", totalWritten);
            }

            private async Task ExportAsync(ExportArguments arguments, Action<DummyEntity> handler)
            {
                var ctx = QueryContext.Default.Unpublished(arguments.Unpublished);

                var (_, service) = Configuration.Setup();

                var client = service.CreateContentsClient<DummyEntity, DummyData>(arguments.Schema);

                var total = 0L;
                var totalRead = 0;
                var currentPage = 0L;

                var consoleTop = Console.CursorTop;

                var handled = new HashSet<Guid>();

                do
                {
                    var query = new ContentQuery
                    {
                        Filter = arguments.Filter,
                        OrderBy = arguments.OrderBy,
                        Search = arguments.FullText,
                        Skip = currentPage * 100,
                        Top = 100
                    };

                    var content = await client.GetAsync(query, ctx);

                    total = content.Total;

                    if (content.Items.Count == 0)
                    {
                        break;
                    }

                    foreach (var entity in content.Items)
                    {
                        if (handled.Add(entity.Id))
                        {
                            totalRead++;

                            handler(entity);

                            Console.WriteLine("> Exported: {0} of {1}.", totalRead, total);
                            Console.SetCursorPosition(0, consoleTop);
                        }
                    }

                    currentPage++;
                }
                while (totalRead < total);

                Console.WriteLine("> Exported: {0} of {1}. Completed.", totalRead, total);
            }

            public enum Format
            {
                CSV,
                JSON
            }

            public interface IImortArgumentBase
            {
                string Schema { get; }

                bool Unpublished { get; }
            }

            [Validator(typeof(ImportArgumentsValidator))]
            public sealed class ImportArguments : IImortArgumentBase, IArgumentModel
            {
                [Argument(Name = "schema", Description = "The name of the schema.")]
                public string Schema { get; set; }

                [Argument(Name = "file", Description = "The path to the file.")]
                public string File { get; set; }

                [Option(LongName = "unpublished", ShortName = "u", Description = "Import unpublished content.")]
                public bool Unpublished { get; set; }

                [Option(LongName = "fields", Description = "Comma separated list of fields to import.")]
                public string Fields { get; set; }

                [Option(LongName = "delimiter", Description = "The csv delimiter.")]
                public string Delimiter { get; set; } = ";";

                [Option(LongName = "format", Description = "Defines the input format.")]
                public Format Format { get; set; }

                public sealed class ImportArgumentsValidator : AbstractValidator<ImportArguments>
                {
                    public ImportArgumentsValidator()
                    {
                        RuleFor(x => x.Delimiter).NotEmpty();
                        RuleFor(x => x.Fields).NotEmpty();
                        RuleFor(x => x.File).NotEmpty();
                        RuleFor(x => x.Schema).NotEmpty();
                    }
                }
            }

            [Validator(typeof(ExportArgumentsValidator))]
            public sealed class ExportArguments : IArgumentModel
            {
                [Argument(Name = "schema", Description = "The name of the schema.")]
                public string Schema { get; set; }

                [Option(LongName = "filter", Description = "Optional filter.")]
                public string Filter { get; set; }

                [Option(LongName = "text", Description = "Optional full text query.")]
                public string FullText { get; set; }

                [Option(LongName = "order", Description = "Optional ordering.")]
                public string OrderBy { get; set; }

                [Option(LongName = "output", Description = "Optional file or folder name. Default: Schema name.")]
                public string Output { get; set; }

                [Option(LongName = "delimiter", Description = "The csv delimiter.")]
                public string Delimiter { get; set; } = ";";

                [Option(LongName = "unpublished", ShortName = "u", Description = "Export unpublished content.")]
                public bool Unpublished { get; set; }

                [Option(LongName = "multiple", ShortName = "m", Description = "Creates one file per content.")]
                public bool FilePerContent { get; set; }

                [Option(LongName = "fields", Description = "Comma separated list of fields. CSV only.")]
                public string Fields { get; set; }

                [Option(LongName = "format", Description = "Defines the output format.")]
                public Format Format { get; set; }

                public sealed class ExportArgumentsValidator : AbstractValidator<ExportArguments>
                {
                    public ExportArgumentsValidator()
                    {
                        RuleFor(x => x.Delimiter).NotEmpty();
                        RuleFor(x => x.Schema).NotEmpty();
                    }
                }
            }

            [Validator(typeof(TestDataArgumentsValidator))]
            public sealed class TestDataArguments : IImortArgumentBase, IArgumentModel
            {
                [Argument(Name = "schema", Description = "The name of the schema.")]
                public string Schema { get; set; }

                [Option(LongName = "unpublished", ShortName = "u", Description = "Import unpublished content.")]
                public bool Unpublished { get; set; }

                [Option(LongName = "count", ShortName = "c", Description = "The number of items.")]
                public int Count { get; set; } = 10;

                [Option(LongName = "file", Description = "The optional path to the file.")]
                public string File { get; set; }

                public sealed class TestDataArgumentsValidator : AbstractValidator<TestDataArguments>
                {
                    public TestDataArgumentsValidator()
                    {
                        RuleFor(x => x.Schema).NotEmpty();
                        RuleFor(x => x.Count).GreaterThan(0);
                    }
                }
            }
        }
    }
}