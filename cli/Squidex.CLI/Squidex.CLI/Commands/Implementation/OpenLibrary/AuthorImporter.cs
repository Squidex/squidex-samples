// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;

#pragma warning disable RECS0082 // Parameter has the same name as a member and hides it
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Squidex.CLI.Commands.Implementation.OpenLibrary
{
    public sealed class AuthorImporter
    {
        private readonly IContentsClient<AuthorContent, AuthorData> client;

        private sealed record CsvRecord(string Id, string Json);

        private sealed record AuthorRecord(string Id, AuthorData Author);

        public AuthorImporter(ISession session)
        {
            client = session.Contents<AuthorContent, AuthorData>("author");
        }

        public async Task ImportAsync(Stream stream)
        {
            var deserializeBlock = new TransformBlock<CsvRecord, AuthorRecord>(x =>
            {
                var data = new AuthorData();

                var json = JObject.Parse(x.Json);

                if (json.TryGetValue("name", out var name))
                {
                    data.Name = GetString(name);
                }

                if (json.TryGetValue("birth_date", out var birthdate))
                {
                    data.Birthdate = GetString(birthdate);
                }

                if (json.TryGetValue("bio", out var bio))
                {
                    data.Bio = GetString(bio);
                }

                if (json.TryGetValue("personal_name", out var personalName))
                {
                    data.PersonalName = GetString(personalName);
                }

                if (json.TryGetValue("wikipedia", out var wikipedia))
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

            Console.Write("Importing (success/failed)...");

            var y = Console.CursorTop;
            var x = Console.CursorLeft;

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

                    Console.SetCursorPosition(x, y);

                    Console.Write("{0}/{1}", totalSuccess, totalFailed);
                }
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                MaxMessagesPerTask = 1,
                BoundedCapacity = Environment.ProcessorCount * 2
            });

            deserializeBlock.LinkTo(batchBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            batchBlock.LinkTo(importBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

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
                            csvReader.GetField(1),
                            csvReader.GetField(4));

                        await deserializeBlock.SendAsync(record);
                    }
                }
            }

            deserializeBlock.Complete();

            await importBlock.Completion;

            Console.WriteLine();
        }

        private static string GetString(JToken value)
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
}
