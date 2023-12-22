// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;
using CommandDotNet;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.ImExport;
using Squidex.CLI.Commands.Implementation.TestData;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.CLI.Commands;

public partial class App
{
    private const string JsonSeparator = "-----------------";

    [Command("contents", Description = "Manage contents.")]
    [Subcommand]
    public sealed class Contents(IConfigurationService configuration, ILogger log)
    {
        [Command("generate", Description = "Generates test data.")]
        public async Task GenerateDummies(GenerateDummiesArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            var taskForSchema = session.Client.Schemas.GetSchemaAsync(arguments.Schema);
            var taskForLanguages = session.Client.Apps.GetLanguagesAsync();

            await Task.WhenAll(
                taskForSchema,
                taskForLanguages);

            var datas = new List<DynamicData>();

            if (arguments.Count > 0)
            {
#pragma warning disable MA0042 // Do not use blocking calls in an async method
                var generator = new TestDataGenerator(taskForSchema.Result, taskForLanguages.Result);
#pragma warning restore MA0042 // Do not use blocking calls in an async method

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

        [Command("enrich-defaults", Description = "Enrich the content items with its defaults.")]
        public async Task EnrichDefaults(EnrichDefaultsArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            var line = log.WriteSameLine();

            var idsRequest = new List<string>();
            var idsTotal = 0;

            async Task BulkUpdateAsync()
            {
                if (idsRequest.Count == 0)
                {
                    return;
                }

                var request = new BulkUpdate
                {
                    Jobs = idsRequest.Select(x => new BulkUpdateJob { Id = x, Type = BulkUpdateType.EnrichDefaults }).ToList()
                };

                await session.Client.DynamicContents(arguments.Schema).BulkUpdateAsync(request);

                idsTotal += idsRequest.Count;
                idsRequest.Clear();

                line.WriteLine("Contents handled: {0}", idsTotal);
            }

            await session.Client.DynamicContents(arguments.Schema).GetAllAsync(async content =>
            {
                idsRequest.Add(content.Id);

                if (idsRequest.Count >= 200)
                {
                    await BulkUpdateAsync();
                }
            }, context: QueryContext.Default.Unpublished(arguments.Unpublished));

            await BulkUpdateAsync();
        }

        [Command("import", Description = "Import the content to a schema.",
            ExtendedHelpText =
@"Use the following format to define fields from the CSV/JSON file:
    - name (for invariant fields)
    - name=other(for invariant fields from another field)
    - name.de=name (for localized fields)
")]
        public async Task Import(ImportArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            if (arguments.Format == ImExportFormat.JSON)
            {
                var converter = new Json2SquidexConverter(arguments.Fields);

                await using (var stream = new FileStream(arguments.File, FileMode.Open, FileAccess.Read))
                {
                    var datas = converter.ReadAsArray(stream);

                    await session.ImportAsync(arguments, log, datas);
                }
            }
            else if (arguments.Format == ImExportFormat.JSON_Separated)
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

        [Command("export", Description = "Export the content for a schema.",
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

            if (arguments.Format == ImExportFormat.JSON && arguments.FilePerContent)
            {
                var folderName = arguments.Output;

                if (string.IsNullOrWhiteSpace(folderName))
                {
                    folderName = $"{arguments.Schema}_{DateTime.UtcNow:yyyy-MM-dd-hh-mm-ss}";
                }

                var folder = Directory.CreateDirectory(folderName);

                await session.ExportAsync(arguments, log, async entity =>
                {
                    var fileName = $"{arguments.Schema}_{entity.Id}.json";
                    var filePath = folder.GetFile(fileName).FullName;

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
            else if (arguments.Format == ImExportFormat.JSON && !arguments.FilePerContent)
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
            else if (arguments.Format == ImExportFormat.JSON_Separated && !arguments.FilePerContent)
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
            else if (arguments.Format == ImExportFormat.CSV && !arguments.FilePerContent)
            {
                var file = OpenFile(".csv");

                var converter = new Squidex2CsvConverter(arguments.Fields);

                await using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    await using (var streamWriter = new StreamWriter(stream))
                    {
                        var csvOptions = new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            Delimiter = arguments.Delimiter
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

        public enum ImExportFormat
        {
            CSV,
            JSON,
            JSON_Separated
        }

        public sealed class ImportArguments : AppArguments, IImportSettings
        {
            [Operand("schema", Description = "The name of the schema.")]
            public string Schema { get; set; }

            [Operand("file", Description = "The path to the file.")]
            public string File { get; set; }

            [Option('u', "unpublished", Description = "Import unpublished content.")]
            public bool Unpublished { get; set; }

            [Option("fields", Description = "Comma separated list of fields to import.")]
            public string Fields { get; set; }

            [Option("delimiter", Description = "The csv delimiter.")]
            public string Delimiter { get; set; } = ";";

            [Option("key", Description = "The key field to use.")]
            public string KeyField { get; set; }

            [Option("format", Description = "Defines the input format.")]
            public ImExportFormat Format { get; set; }

            public sealed class Validator : AbstractValidator<ImportArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.File).NotEmpty();
                    RuleFor(x => x.Schema).NotEmpty();

                    When(x => x.Format == ImExportFormat.CSV, () =>
                    {
                        RuleFor(x => x.Delimiter).NotEmpty();
                        RuleFor(x => x.Fields).NotEmpty();
                    });
                }
            }
        }

        public sealed class ExportArguments : AppArguments, IExportSettings
        {
            [Operand("schema", Description = "The name of the schema.")]
            public string Schema { get; set; }

            [Option("filter", Description = "Optional filter.")]
            public string Filter { get; set; }

            [Option("text", Description = "Optional full text query.")]
            public string FullText { get; set; }

            [Option("order", Description = "Optional ordering.")]
            public string OrderBy { get; set; }

            [Option("output", Description = "Optional file or folder name. Default: Schema name.")]
            public string Output { get; set; }

            [Option("delimiter", Description = "The csv delimiter.")]
            public string Delimiter { get; set; } = ";";

            [Option('u', "unpublished", Description = "Export unpublished content.")]
            public bool Unpublished { get; set; }

            [Option('m', "multiple", Description = "Creates one file per content.")]
            public bool FilePerContent { get; set; }

            [Option("fields", Description = "Comma separated list of fields. CSV only.")]
            public string Fields { get; set; }

            [Option("full", Description = "Write full entities, not only data when exporting as CSV. Default: false.")]
            public bool FullEntities { get; set; }

            [Option("format", Description = "Defines the output format.")]
            public ImExportFormat Format { get; set; }

            public sealed class Validator : AbstractValidator<ExportArguments>
            {
                public Validator()
                {
                    When(x => x.Format == ImExportFormat.CSV, () =>
                    {
                        RuleFor(x => x.Delimiter).NotEmpty();
                        RuleFor(x => x.Fields).NotEmpty();
                    });

                    RuleFor(x => x.Schema).NotEmpty();
                }
            }
        }

        public sealed class GenerateDummiesArguments : AppArguments, IImportSettings
        {
            [Operand("schema", Description = "The name of the schema.")]
            public string Schema { get; set; }

            [Option('u', "unpublished", Description = "Import unpublished content.")]
            public bool Unpublished { get; set; }

            [Option('c', "count", Description = "The number of items.")]
            public int Count { get; set; } = 10;

            [Option("file", Description = "The optional path to the file.")]
            public string File { get; set; }

            string? IImportSettings.KeyField => null;

            public sealed class Validator : AbstractValidator<GenerateDummiesArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Schema).NotEmpty();
                    RuleFor(x => x.Count).GreaterThan(0);
                }
            }
        }

        public sealed class EnrichDefaultsArguments : AppArguments
        {
            [Operand("schema", Description = "The name of the schema.")]
            public string Schema { get; set; }

            [Option('u', "unpublished", Description = "Handle unpublished content.")]
            public bool Unpublished { get; set; }

            public sealed class Validator : AbstractValidator<EnrichDefaultsArguments>
            {
            }
        }
    }
}
