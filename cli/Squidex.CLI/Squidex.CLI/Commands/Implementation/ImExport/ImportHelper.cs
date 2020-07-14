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
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;

namespace Squidex.CLI.Commands.Implementation.ImExport
{
    public static class ImportHelper
    {
        public static async Task ImportAsync(this ISession session, IImportSettings setting, ILogger log, IEnumerable<DummyData> datas)
        {
            var contents = session.Contents(setting.Schema);

            var totalWritten = 0;

            using (var logLine = log.WriteSameLine())
            {
                foreach (var data in datas)
                {
                    await contents.CreateAsync(data, !setting.Unpublished);

                    totalWritten++;

                    logLine.WriteLine("> Imported: {0}.", totalWritten);
                }
            }

            log.WriteLine("> Imported: {0}. Completed.", totalWritten);
        }

        public static IEnumerable<DummyData> Read(this Csv2SquidexConverter converter, Stream stream, string delimiter)
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

        public static IEnumerable<DummyData> ReadAsArray(this Json2SquidexConverter converter, Stream stream)
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

        public static IEnumerable<DummyData> ReadAsSeparatedObjects(this Json2SquidexConverter converter, Stream stream, string separator)
        {
            var sb = new StringBuilder();

            string line;

            using (var streamReader = new StreamReader(stream))
            {
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
}
