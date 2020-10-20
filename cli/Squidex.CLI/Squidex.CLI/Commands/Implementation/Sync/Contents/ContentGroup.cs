// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public sealed class ContentGroup : IEquatable<ContentGroup>
    {
        public string SchemaName { get; }

        public List<ContentModel> Contents { get; }

        public HashSet<string> Dependencies { get; } = new HashSet<string>();

        public ContentGroup(string schemaName, IEnumerable<ContentModel> contents)
        {
            SchemaName = schemaName;

            Contents = contents.ToList();

            foreach (var content in contents)
            {
                if (content.References != null)
                {
                    foreach (var reference in content.References.Values)
                    {
                        Dependencies.Add(reference.Schema);
                    }
                }
            }
        }

        public void ClearLanguages(SyncOptions options)
        {
            if (options.Languages?.Length > 0)
            {
                var allowedLanguages = options.Languages.ToHashSet();

                var toClear = new List<string>();

                foreach (var content in Contents)
                {
                    foreach (var field in content.Data.Values)
                    {
                        foreach (var language in field.Keys)
                        {
                            if (language != "iv" && !allowedLanguages.Contains(language))
                            {
                                toClear.Add(language);
                            }
                        }

                        if (toClear.Count > 0)
                        {
                            foreach (var language in toClear)
                            {
                                field.Remove(language);
                            }

                            toClear.Clear();
                        }
                    }
                }
            }
        }

        public async Task ResolveReferencesAsync(ISession session, ILogger log, ReferenceCache cache)
        {
            foreach (var content in Contents.ToList())
            {
                if (content.References?.Any() == true)
                {
                    await ResolveContentAsync(content, session, log, cache);
                }
            }
        }

        public async Task UpsertAsync(ISession session, ILogger log)
        {
            var client = session.Contents(SchemaName);

            var request = new BulkUpdate
            {
                Jobs = Contents.Select(x => new BulkUpdateJob { Query = new { filter = x.Filter }, Data = x.Data }).ToList()
            };

            var results = await client.BulkUpdateAsync(request);

            var i = 0;

            foreach (var result in results)
            {
                var content = Contents[i];

                log.StepStart($"{content.File} / {content.Ref}");

                if (result.ContentId != null)
                {
                    log.StepSuccess();
                }
                else if (result.Error != null)
                {
                    log.StepFailed(result.Error.ToString());
                }
                else
                {
                    log.StepSkipped("Unknown Reason");
                }

                i++;
            }
        }

        private async Task ResolveContentAsync(ContentModel content, ISession session, ILogger log, ReferenceCache cache)
        {
            var resolvedReferences = new Dictionary<string, string>();

            foreach (var (key, value) in content.References.ToList())
            {
                var schema = value.Schema;

                var filter = JObject.FromObject(value.Filter);
                var filterJson = filter.ToString();

                if (cache.TryGetValue((schema, filterJson), out var id))
                {
                    resolvedReferences[key] = id;
                    continue;
                }

                var client = session.Contents(value.Schema);

                var query = new { filter, take = 1 };

                var references = await client.GetAsync(new ContentQuery { JsonQuery = query }, QueryContext.Default.Unpublished(true));

                if (references.Total == 1)
                {
                    id = references.Items[0].Id;

                    resolvedReferences[key] = id;

                    cache[(schema, filterJson)] = id;
                }
                else if (references.Total > 1)
                {
                    log.WriteLine("Skipping {0} / {1} reference {2} not unique.", content.File, content.Ref, key);

                    Contents.Remove(content);
                    return;
                }
                else
                {
                    log.WriteLine("Skipping {0} / {1} reference {2} not resolved.", content.File, content.Ref, key);

                    Contents.Remove(content);
                    return;
                }
            }

            foreach (var field in content.Data.Values)
            {
                foreach (var value in field.Values)
                {
                    UpdateData(value, resolvedReferences);
                }
            }
        }

        private void UpdateData(JToken data, Dictionary<string, string> resolvedReferences)
        {
            if (data is JArray array)
            {
                for (var i = 0; i < array.Count; i++)
                {
                    var value = array[i];

                    if (value.Type == JTokenType.String)
                    {
                        if (resolvedReferences.TryGetValue(value.Value<string>(), out var id))
                        {
                            array[i] = id;
                        }
                    }
                    else
                    {
                        UpdateData(value, resolvedReferences);
                    }
                }
            }
            else if (data is JObject obj)
            {
                ICollection<KeyValuePair<string, JToken>> list = obj;

                foreach (var (key, value) in list.ToList())
                {
                    if (value.Type == JTokenType.String)
                    {
                        if (resolvedReferences.TryGetValue(value.Value<string>(), out var id))
                        {
                            data[key] = id;
                        }
                    }
                    else
                    {
                        UpdateData(value, resolvedReferences);
                    }
                }
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContentGroup);
        }

        public bool Equals(ContentGroup other)
        {
            return string.Equals(other?.SchemaName, SchemaName);
        }

        public override int GetHashCode()
        {
            return SchemaName.GetHashCode();
        }

        public override string ToString()
        {
            return SchemaName;
        }
    }
}
