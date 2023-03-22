// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;
using System.Threading.Tasks.Dataflow;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.ClientLibrary;

#pragma warning disable RECS0082 // Parameter has the same name as a member and hides it
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Squidex.CLI.Commands.Implementation.OpenLibrary;

public sealed class AuthorImporter
{
    private readonly IContentsClient<AuthorContent, AuthorData> client;
    private readonly ILogger log;

    private sealed record CsvRecord(string Id, string Json);

    private sealed record AuthorRecord(string Id, AuthorData Author);

    public AuthorImporter(ISession session, ILogger log)
    {
        client = session.Client.Contents<AuthorContent, AuthorData>("author");

        this.log = log;
    }

    public async Task ImportAsync(Stream stream)
    {
        var deserializeBlock = new TransformBlock<CsvRecord, AuthorRecord>(x =>
        {
            var data = new AuthorData();

            var json = JObject.Parse(x.Json);

            if (json.TryGetValue("name", StringComparison.Ordinal, out var name))
            {
                data.Name = GetString(name);
            }

            if (json.TryGetValue("birth_date", StringComparison.Ordinal, out var birthdate))
            {
                data.Birthdate = GetString(birthdate);
            }

            if (json.TryGetValue("bio", StringComparison.Ordinal, out var bio))
            {
                data.Bio = GetString(bio);
            }

            if (json.TryGetValue("personal_name", StringComparison.Ordinal, out var personalName))
            {
                data.PersonalName = GetString(personalName);
            }

            if (json.TryGetValue("wikipedia", StringComparison.Ordinal, out var wikipedia))
            {
                data.Wikipedia = GetString(wikipedia);
            }

            return new AuthorRecord(x.Id, data);
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            MaxMessagesPerTask = 1,
            BoundedCapacity = 100
        });

        var batchBlock = new BatchBlock<AuthorRecord>(100, new GroupingDataflowBlockOptions
        {
            BoundedCapacity = 100
        });

        var totalFailed = 0;
        var totalSuccess = 0;

        using (var logLine = log.WriteSameLine())
        {
            if (logLine.CanWriteToSameLine)
            {
                logLine.WriteLine("Importing (success/failed)...");
            }

            var lockObject = new object();

            var importBlock = new ActionBlock<AuthorRecord[]>(async authors =>
            {
                var request = new BulkUpdate
                {
                    OptimizeValidation = true,
                    DoNotScript = true,
                    DoNotValidate = false,
                    DoNotValidateWorkflow = true,
                    Jobs = authors.Select(x =>
                    {
                        return new BulkUpdateJob
                        {
                            Id = x.Id,
                            Data = x.Author,
                            Type = BulkUpdateType.Upsert
                        };
                    }).ToList()
                };

                var response = await client.BulkUpdateAsync(request);

                lock (lockObject)
                {
                    totalFailed += response.Count(x => x.Error != null);
                    totalSuccess += response.Count(x => x.Error == null);

                    logLine.WriteLine("Importing (success/failed)...{0}/{1}", totalSuccess, totalFailed);
                }
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                MaxMessagesPerTask = 1,
                BoundedCapacity = Environment.ProcessorCount * 2
            });

            deserializeBlock.BidirectionalLinkTo(batchBlock);

            batchBlock.BidirectionalLinkTo(importBlock);

            using (var streamReader = new StreamReader(stream))
            {
                var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    BadDataFound = null,
                    Delimiter = "\t"
                };

                using (var csvReader = new CsvReader(streamReader, configuration))
                {
                    while (await csvReader.ReadAsync())
                    {
                        var record = new CsvRecord(
                            csvReader.GetField(1)!,
                            csvReader.GetField(4)!);

                        await deserializeBlock.SendAsync(record);
                    }
                }
            }

            deserializeBlock.Complete();

            await importBlock.Completion;

            if (!logLine.CanWriteToSameLine)
            {
                log.WriteLine("Importing (success/failed)...{0}/{1}", totalSuccess, totalFailed);
            }
        }

        log.WriteLine();
    }

    private static string? GetString(JToken value)
    {
        switch (value.Type)
        {
            case JTokenType.String:
                return value.ToString();
            case JTokenType.Object:
                return value.Value<string>("value");
            default:
                return null;
        }
    }
}
