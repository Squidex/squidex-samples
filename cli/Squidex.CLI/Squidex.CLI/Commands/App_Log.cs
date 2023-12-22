// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;
using CommandDotNet;
using ConsoleTables;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using Squidex.CLI.Commands.Implementation;

#pragma warning disable MA0048 // File name must match type name
#pragma warning disable RECS0014 // If all fields, properties and methods members are static, the class can be made static.

namespace Squidex.CLI.Commands;

public sealed partial class App
{
    [Command("log", Description = "Analyze request log.")]
    [Subcommand]
    public sealed class Log(ILogger log)
    {
        [Command("analyze", Description = "Analyzes request log files.")]
        public void Analyze(AnalyzeArguments arguments)
        {
            using (var reader = new StreamReader(arguments.File))
            {
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    Delimiter = "|"
                }))
                {
                    var records = csv.GetRecords<Record>().ToList();

                    var groups = records.GroupBy(x => $"{x.RequestMethod} {x.RequestPath}").Select(x => new
                    {
                        Path = x.Key,
                        TotalCalls = x.Count(),
                        TotalCosts = Math.Round(x.Sum(x => x.Costs)),
                        AveragePerformance = x.Average(x => x.RequestElapsedMs),
                        IsAsset = x.Key.StartsWith("GET /api/assets", StringComparison.OrdinalIgnoreCase)
                    }).ToList();

                    log.WriteLine("Most used requests:");

                    var table = new ConsoleTable("Path", "Count");

                    foreach (var item in groups.OrderByDescending(x => x.TotalCalls).Take(20))
                    {
                        table.AddRow(item.Path, item.TotalCalls);
                    }

                    log.WriteLine(table.ToString());
                    log.WriteLine();
                    log.WriteLine("Most expensive requests:");

                    table = new ConsoleTable("Path", "Costs");

                    foreach (var item in groups.OrderByDescending(x => x.TotalCosts).Take(20))
                    {
                        table.AddRow(item.Path, item.TotalCosts);
                    }

                    log.WriteLine(table.ToString());
                    log.WriteLine();
                    log.WriteLine("Slowest requests (without assets)");

                    table = new ConsoleTable("Path", "Average Response Time");

                    foreach (var item in groups.Where(x => !x.IsAsset).OrderByDescending(x => x.AveragePerformance).Take(20))
                    {
                        table.AddRow(item.Path, item.AveragePerformance);
                    }

                    log.WriteLine(table.ToString());
                    log.WriteLine();
                    log.WriteLine("Summary");

                    table = new ConsoleTable("Key", "Value");

                    table.AddRow("Total calls", groups.Sum(x => x.TotalCalls));
                    table.AddRow("Total costs", groups.Sum(x => x.TotalCosts));
                    table.AddRow("Average performance", groups.Average(x => x.AveragePerformance));
                    table.AddRow("Average performance (without assets)", groups.Where(x => !x.IsAsset).Average(x => x.AveragePerformance));

                    table.Write();
                }
            }
        }

        public sealed class AnalyzeArguments : IArgumentModel
        {
            [Operand("file", Description = "The source file.")]
            public string File { get; set; }

            public sealed class Validator : AbstractValidator<AnalyzeArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.File).NotEmpty();
                }
            }
        }

        private sealed class Record
        {
            public string Timestamp { get; set; }

            public string RequestPath { get; set; }

            public string RequestMethod { get; set; }

            public double RequestElapsedMs { get; set; }

            public double Costs { get; set; }

            public string AuthClientId { get; set; }

            public string AuthUserId { get; set; }
        }
    }
}
