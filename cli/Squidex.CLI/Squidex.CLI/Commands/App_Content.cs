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
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using FluentValidation.Attributes;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.ImExport;
using Squidex.CLI.Commands.Implementation.TestData;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        private const string JsonSeparator = "-----------------";

        [Command(Name = "content", Description = "Manage contents.")]
        [SubCommand]
        public sealed class Content
        {
            private readonly IConfigurationService configuration;
            private readonly ILogger log;

            public Content(IConfigurationService configuration, ILogger log)
            {
                this.configuration = configuration;

                this.log = log;
            }

            [Command(Name = "test-data", Description = "Generates test data.")]
            public async Task TestData(TestDataArguments arguments)
            {
                var session = configuration.StartSession(arguments.App);

                var taskForSchema = session.Schemas.GetSchemaAsync(session.App, arguments.Schema);
                var taskForLanguages = session.Apps.GetLanguagesAsync(session.App);

                await Task.WhenAll(
                    taskForSchema,
                    taskForLanguages);

                var datas = new List<DynamicData>();

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
                    await using (var stream = new FileStream(arguments.File, FileMode.Create, FileAccess.Write))
                    {
                        await stream.WriteJsonAsync(datas);
                    }
                }
                else
                {
                    await session.ImportAsync(arguments, log, datas);
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
                var session = configuration.StartSession(arguments.App);

                if (arguments.Format == Format.JSON)
                {
                    var converter = new Json2SquidexConverter(arguments.Fields);

                    await using (var stream = new FileStream(arguments.File, FileMode.Open, FileAccess.Read))
                    {
                        var datas = converter.ReadAsArray(stream);

                        await session.ImportAsync(arguments, log, datas);
                    }
                }
                else if (arguments.Format == Format.JSON_Separated)
                {
                    var converter = new Json2SquidexConverter(arguments.Fields);

                    await using (var stream = new FileStream(arguments.File, FileMode.Open, FileAccess.Read))
                    {
                        var datas = converter.ReadAsSeparatedObjects(stream, JsonSeparator);

                        await session.ImportAsync(arguments, log, datas);
                    }
                }
                else
                {
                    var converter = new Csv2SquidexConverter(arguments.Fields);

                    await using (var stream = new FileStream(arguments.File, FileMode.Open, FileAccess.Read))
                    {
                        var datas = converter.Read(stream, arguments.Delimiter);

                        await session.ImportAsync(arguments, log, datas);
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
                var session = configuration.StartSession(arguments.App);

                string OpenFile(string extension)
                {
                    var file = arguments.Output;

                    if (string.IsNullOrWhiteSpace(file))
                    {
                        file = $"{arguments.Schema}_{DateTime.UtcNow:yyyy-MM-dd-hh-mm-ss}{extension}";
                    }

                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }

                    return file;
                }

                if (arguments.Format == Format.JSON && arguments.FilePerContent)
                {
                    var folder = arguments.Output;

                    if (string.IsNullOrWhiteSpace(folder))
                    {
                        folder = $"{arguments.Schema}_{DateTime.UtcNow:yyyy-MM-dd-hh-mm-ss}";
                    }

                    Directory.CreateDirectory(folder);

                    await session.ExportAsync(arguments, log, async entity =>
                    {
                        var fileName = $"{arguments.Schema}_{entity.Id}.json";
                        var filePath = Path.Combine(folder, fileName);

                        if (arguments.FullEntities)
                        {
                            await Helper.WriteJsonToFileAsync(entity, filePath);
                        }
                        else
                        {
                            await Helper.WriteJsonToFileAsync(entity.Data, filePath);
                        }
                    });
                }
                else if (arguments.Format == Format.JSON && !arguments.FilePerContent)
                {
                    var file = OpenFile(".json");

                    var allRecords = new List<DynamicContent>();

                    await session.ExportAsync(arguments, log, entity =>
                    {
                        allRecords.Add(entity);

                        return Task.CompletedTask;
                    });

                    if (arguments.FullEntities)
                    {
                        await Helper.WriteJsonToFileAsync(allRecords, file);
                    }
                    else
                    {
                        await Helper.WriteJsonToFileAsync(allRecords.Select(x => x.Data), file);
                    }
                }
                else if (arguments.Format == Format.JSON_Separated && !arguments.FilePerContent)
                {
                    var file = OpenFile(".json");

                    await using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write))
                    {
                        await using (var writer = new StreamWriter(stream))
                        {
                            await session.ExportAsync(arguments, log, async entity =>
                            {
                                if (arguments.FullEntities)
                                {
                                    await writer.WriteJsonAsync(entity);
                                }
                                else
                                {
                                    await writer.WriteJsonAsync(entity.Data);
                                }

                                await writer.WriteLineAsync();
                                await writer.WriteLineAsync(JsonSeparator);
                            });
                        }
                    }
                }
                else if (arguments.Format == Format.CSV && !arguments.FilePerContent)
                {
                    var file = OpenFile(".csv");

                    var converter = new Squidex2CsvConverter(arguments.Fields);

                    await using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write))
                    {
                        await using (var streamWriter = new StreamWriter(stream))
                        {
                            var csvOptions = new CsvConfiguration(CultureInfo.InvariantCulture)
                            {
                                Delimiter = ";"
                            };

                            await using (var writer = new CsvWriter(streamWriter, csvOptions))
                            {
                                foreach (var fieldName in converter.FieldNames)
                                {
                                    writer.WriteField(fieldName);
                                }

                                await writer.NextRecordAsync();

                                await session.ExportAsync(arguments, log, async entity =>
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

                                    await writer.NextRecordAsync();
                                });
                            }
                        }
                    }
                }
                else
                {
                    throw new CLIException("Multiple files are not supported for this format.");
                }
            }

            public enum Format
            {
                CSV,
                JSON,
                JSON_Separated
            }

            [Validator(typeof(Validator))]
            public sealed class ImportArguments : AppArguments, IImportSettings
            {
                [Operand(Name = "schema", Description = "The name of the schema.")]
                public string Schema { get; set; }

                [Operand(Name = "file", Description = "The path to the file.")]
                public string File { get; set; }

                [Option(LongName = "unpublished", ShortName = "u", Description = "Import unpublished content.")]
                public bool Unpublished { get; set; }

                [Option(LongName = "fields", Description = "Comma separated list of fields to import.")]
                public string Fields { get; set; }

                [Option(LongName = "delimiter", Description = "The csv delimiter.")]
                public string Delimiter { get; set; } = ";";

                [Option(LongName = "format", Description = "Defines the input format.")]
                public Format Format { get; set; }

                public sealed class Validator : AbstractValidator<ImportArguments>
                {
                    public Validator()
                    {
                        RuleFor(x => x.File).NotEmpty();
                        RuleFor(x => x.Schema).NotEmpty();

                        When(x => x.Format == Format.CSV, () =>
                        {
                            RuleFor(x => x.Delimiter).NotEmpty();
                            RuleFor(x => x.Fields).NotEmpty();
                        });
                    }
                }
            }

            [Validator(typeof(Validator))]
            public sealed class ExportArguments : AppArguments, IExportSettings
            {
                [Operand(Name = "schema", Description = "The name of the schema.")]
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

                [Option(LongName = "full", Description = "Write full entities, not only data when exporting as CSV. Default: false.")]
                public bool FullEntities { get; set; }

                [Option(LongName = "format", Description = "Defines the output format.")]
                public Format Format { get; set; }

                public sealed class Validator : AbstractValidator<ExportArguments>
                {
                    public Validator()
                    {
                        When(x => x.Format == Format.CSV, () =>
                        {
                            RuleFor(x => x.Delimiter).NotEmpty();
                            RuleFor(x => x.Fields).NotEmpty();
                        });

                        RuleFor(x => x.Schema).NotEmpty();
                    }
                }
            }

            [Validator(typeof(Validator))]
            public sealed class TestDataArguments : AppArguments, IImportSettings
            {
                [Operand(Name = "schema", Description = "The name of the schema.")]
                public string Schema { get; set; }

                [Option(LongName = "unpublished", ShortName = "u", Description = "Import unpublished content.")]
                public bool Unpublished { get; set; }

                [Option(LongName = "count", ShortName = "c", Description = "The number of items.")]
                public int Count { get; set; } = 10;

                [Option(LongName = "file", Description = "The optional path to the file.")]
                public string File { get; set; }

                public sealed class Validator : AbstractValidator<TestDataArguments>
                {
                    public Validator()
                    {
                        RuleFor(x => x.Schema).NotEmpty();
                        RuleFor(x => x.Count).GreaterThan(0);
                    }
                }
            }
        }
    }
}
