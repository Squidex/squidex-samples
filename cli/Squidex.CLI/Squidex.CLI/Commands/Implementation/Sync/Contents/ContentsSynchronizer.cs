// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public sealed class ContentsSynchronizer : ISynchronizer
    {
        private readonly ILogger log;

        public string Name => "contents";

        public ContentsSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public Task ExportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            log.WriteLine("Not supported");

            return Task.CompletedTask;
        }

        public async Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var contentsBySchema =
                GetContentFiles(directoryInfo)
                    .Select(x => ReadContents(jsonHelper, x))
                    .SelectMany(x => x.Contents).GroupBy(x => x.Schema)
                    .Select(x => new ContentGroup(x.Key, x))
                    .ToDictionary(x => x.SchemaName);

            var edges = new HashSet<(ContentGroup From, ContentGroup To)>();

            foreach (var from in contentsBySchema.Values)
            {
                foreach (var dependency in from.Dependencies)
                {
                    if (contentsBySchema.TryGetValue(dependency, out var to))
                    {
                        edges.Add((to, from));
                    }
                }
            }

            var sortOrder = TopologicalSort.Sort(new HashSet<ContentGroup>(contentsBySchema.Values), edges);

            if (sortOrder == null)
            {
                throw new SquidexException("Dependencies between content items have a cycle, cannot calculate best sync order.");
            }

            var cache = new ReferenceCache();

            foreach (var (schema, group) in contentsBySchema)
            {
                await log.DoSafeLineAsync($"Schema {schema}: Resolving references", () =>
                {
                    group.ClearLanguages(options);

                    return group.ResolveReferencesAsync(session, log, cache);
                });

                await log.DoSafeLineAsync($"Schema {schema}: Inserting", () =>
                {
                    return group.UpsertAsync(session, log);
                });
            }
        }

        private ContentsModel ReadContents(JsonHelper jsonHelper, FileInfo file)
        {
            var result = jsonHelper.Read<ContentsModel>(file, log);

            var index = 0;

            foreach (var content in result.Contents)
            {
                content.File = file.Name;
                content.Ref = index.ToString();

                index++;
            }

            return result;
        }

        private IEnumerable<FileInfo> GetContentFiles(DirectoryInfo directoryInfo)
        {
            foreach (var file in directoryInfo.GetFiles("contents/*.json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }
        }

        public async Task GenerateSchemaAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<ContentsModel>(directoryInfo, "contents.json");

            var sample = new ContentsModel
            {
                Contents = new List<ContentModel>
                {
                    new ContentModel
                    {
                        Schema = "my-schema",
                        Filter = JObject.FromObject(new
                        {
                            Path = "data.id.iv",
                            Op = "eq",
                            Value = 1
                        }),
                        Data = new Dictionary<string, Dictionary<string, JToken>>
                        {
                            ["id"] = new Dictionary<string, JToken>
                            {
                                ["iv"] = 1
                            },
                            ["text"] = new Dictionary<string, JToken>
                            {
                                ["iv"] = "Hello Squidex"
                            }
                        }
                    }
                }
            };

            await jsonHelper.WriteWithSchema(directoryInfo, "contents/__contents.json", sample, "../__json/contents");
        }
    }
}
