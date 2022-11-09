// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.ImExport;

public static class ImportHelper
{
    public static async Task ImportAsync(this ISession session, IImportSettings setting, ILogger log, IEnumerable<DynamicData> datas)
    {
        var contents = session.Contents(setting.Schema);

        var totalWritten = 0;

        using (var logLine = log.WriteSameLine())
        {
            var keyField = setting.KeyField;

            var update = new BulkUpdate
            {
                Jobs = new List<BulkUpdateJob>(),
                DoNotScript = false,
                DoNotValidate = false,
                Publish = !setting.Unpublished
            };

            const string op = "eq";

            foreach (var batch in datas.Batch(50))
            {
                update.Jobs.Clear();

                foreach (var data in batch)
                {
                    var job = new BulkUpdateJob
                    {
                        Data = data,
                    };

                    if (!string.IsNullOrWhiteSpace(keyField))
                    {
                        if (!data.TryGetValue(keyField, out var temp) || temp is not JObject obj || !obj.TryGetValue("iv", StringComparison.Ordinal, out var value))
                        {
                            throw new InvalidOperationException($"Cannot find key '{keyField}' in data.");
                        }

                        job.Query = new
                        {
                            filter = new
                            {
                                path = $"data.{keyField}.iv",
                                op,
                                value,
                            }
                        };

                        job.Type = BulkUpdateType.Upsert;
                    }
                    else
                    {
                        job.Type = BulkUpdateType.Create;
                    }

                    update.Jobs.Add(job);
                }

                var result = await contents.BulkUpdateAsync(update);

                var error = result.Find(x => x.Error != null)?.Error;

                if (error != null)
                {
                    throw new SquidexManagementException<ErrorDto>(error.Message, error.StatusCode, null, null, error, null);
                }

                totalWritten += update.Jobs.Count;

                if (logLine.CanWriteToSameLine)
                {
                    logLine.WriteLine("> Imported: {0}.", totalWritten);
                }
            }
        }

        log.WriteLine("> Imported: {0}. Completed.", totalWritten);
    }

    public static IEnumerable<DynamicData> Read(this Csv2SquidexConverter converter, Stream stream, string delimiter)
    {
        using (var streamReader = new StreamReader(stream))
        {
            var csvOptions = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter
            };

            using (var reader = new CsvReader(streamReader, csvOptions))
            {
                foreach (var data in converter.ReadAll(reader))
                {
                    yield return data;
                }
            }
        }
    }

    public static IEnumerable<DynamicData> ReadAsArray(this Json2SquidexConverter converter, Stream stream)
    {
        using (var streamReader = new StreamReader(stream))
        {
            using (var reader = new JsonTextReader(streamReader))
            {
                foreach (var data in converter.ReadAll(reader))
                {
                    yield return data;
                }
            }
        }
    }

    public static IEnumerable<DynamicData> ReadAsSeparatedObjects(this Json2SquidexConverter converter, Stream stream, string separator)
    {
        var sb = new StringBuilder();

        using (var streamReader = new StreamReader(stream))
        {
            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line.Equals(separator, StringComparison.OrdinalIgnoreCase))
                {
                    using (var stringReader = new StringReader(sb.ToString()))
                    {
                        using (var reader = new JsonTextReader(stringReader))
                        {
                            yield return converter.ReadOne(reader);
                        }
                    }

                    sb.Clear();
                }
                else
                {
                    sb.AppendLine(line);
                }
            }
        }
    }
}
