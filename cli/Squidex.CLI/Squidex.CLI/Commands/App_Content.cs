// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet;
using CommandDotNet.Attributes;
using CsvHelper;
using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json.Linq;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [ApplicationMetadata(Name = "content", Description = "Manage content.")]
        [SubCommand]
        public class Content
        {
            [InjectProperty]
            public IConfigurationService Configuration { get; set; }

            [ApplicationMetadata(Name = "export", Description = "Export the content for a schema.",
                ExtendedHelpText =
@"Use the following format to define fields for CSV:
    - id
    - createdBy
    - created
    - lastModifiedBy
    - lastModified
    - version
    - data.name (for invariant fields)
    - Name=data.name (for invariant fields with alias)
    - data.name.de (for localized fields)
    - Name (German)=data.name.de (for localized fields with alias)
")]
            public async Task Export(ExportArguments arguments)
            {
                var ctx = QueryContext.Default.Unpublished(arguments.Unpublished);

                var client = Configuration.GetClient().Client.GetClient<DummyEntity, DummyData>(arguments.Schema);

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

                    var fields = GetFields(arguments);

                    using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write))
                    {
                        using (var streamWriter = new StreamWriter(stream))
                        {
                            var csvOptions = new CsvHelper.Configuration.Configuration
                            {
                                Delimiter = ";"
                            };

                            using (var writer = new CsvWriter(streamWriter, csvOptions))
                            {
                                foreach (var field in fields)
                                {
                                    writer.WriteField(field.Name);
                                }

                                writer.NextRecord();

                                await ExportAsync(arguments, entity =>
                                {
                                    foreach (var field in fields)
                                    {
                                        var value = GetValue(entity, field.Path);

                                        if (value is string s)
                                        {
                                            writer.WriteField(s.Replace("\n", "\\n"), true);
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

            private static List<(string Name, string[] Path)> GetFields(ExportArguments arguments)
            {
                var fields = new List<(string Name, string[] Path)>();

                foreach (var item in arguments.Fields.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = item.Split('=');

                    string[] GetPath(string value)
                    {
                        var path = value.Split('.', StringSplitOptions.RemoveEmptyEntries);

                        if (path.Length == 2 && string.Equals(path[0], "Data", StringComparison.OrdinalIgnoreCase))
                        {
                            return path.Union(Enumerable.Repeat("iv", 1)).ToArray();
                        }

                        return path;
                    }

                    if (parts.Length == 1)
                    {
                        fields.Add((parts[0], GetPath(parts[0])));
                    }
                    else if (parts.Length == 2)
                    {
                        fields.Add((parts[0], GetPath(parts[1])));
                    }
                    else
                    {
                        throw new SquidexException("Field definition not valid.");
                    }
                }

                return fields;
            }

            private async Task ExportAsync(ExportArguments arguments, Action<DummyEntity> handler)
            {
                var ctx = QueryContext.Default.Unpublished(arguments.Unpublished);

                var client = Configuration.GetClient().Client.GetClient<DummyEntity, DummyData>(arguments.Schema);

                var total = 0L;
                var totalRead = 0;
                var currentPage = 0L;

                var consoleTop = Console.CursorTop;

                var handled = new HashSet<string>();

                do
                {
                    var content = await client.GetAsync(currentPage * 100, 100, arguments.Filter, arguments.OrderBy, arguments.FullText, ctx);

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

            private object GetValue(object current, string[] path)
            {
                foreach (var element in path)
                {
                    if (current is JObject obj)
                    {
                        if (obj.TryGetValue(element, out var temp))
                        {
                            current = temp;
                        }
                        else
                        {
                            return "<INVALID>";
                        }
                    }
                    else if (current is IDictionary dict)
                    {
                        if (dict.Contains(element))
                        {
                            current = dict[element];
                        }
                        else
                        {
                            return "<INVALID>";
                        }
                    }
                    else if (current is JArray arr)
                    {
                        if (int.TryParse(element, out var idx) && idx > 0 && idx < arr.Count)
                        {
                            return arr[idx];
                        }
                        else
                        {
                            return "<INVALID>";
                        }
                    }
                    else
                    {
                        var property = current.GetType().GetProperties().FirstOrDefault(x => x.CanRead && string.Equals(x.Name, element, StringComparison.OrdinalIgnoreCase));

                        if (property != null)
                        {
                            current = property.GetValue(current);
                        }
                        else
                        {
                            return "<INVALID>";
                        }
                    }
                }

                if (current is JValue value)
                {
                    return value.Value;
                }
                else if (current?.GetType().IsClass == true)
                {
                    return current.JsonString();
                }
                else
                {
                    return current;
                }
            }

            public enum Format
            {
                CSV,
                JSON
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
                        RuleFor(x => x.Schema).NotEmpty();
                    }
                }
            }
        }
    }
}