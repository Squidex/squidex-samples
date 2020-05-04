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
    public sealed class ContentGroup
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

        public async Task ResolveReferencesAsync(ISession session, ILogger log, ReferenceCache cache)
        {
            foreach (var content in Contents.ToList())
            {
                if (content.References?.Any() == true)
                {
                    await ResovleContntAsync(content, session, log, cache);
                }
            }
        }

        public async Task UpsertAsync(ISession session, ILogger log, ReferenceCache cache)
        {
            var client = session.Contents(SchemaName);

            var request = new BulkUpdate
            {
                Jobs = Contents.Select(x => new BulkUpdateJob { Query = x.Query, Data = x.Data }).ToList()
            };

            var results = await client.BulkUpdateAsync(request);

            var i = 0;

            foreach (var result in results)
            {
                var content = Contents[i];

                log.StepStart($"Content {content.File} / {content.Ref}");

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

        private async Task ResovleContntAsync(ContentModel content, ISession session, ILogger log, ReferenceCache cache)
        {
            var resolvedReferences = new Dictionary<string, Guid>();

            foreach (var (key, value) in content.References.ToList())
            {
                var schema = value.Schema;

                var queryObject = JObject.FromObject(value.Query);
                var queryJson = queryObject.ToString();

                if (cache.TryGetValue((schema, queryJson), out var id))
                {
                    resolvedReferences[key] = id;
                    continue;
                }

                var client = session.Contents(value.Schema);

                queryObject["take"] = 1;

                var references = await client.GetAsync(new ContentQuery { JsonQuery = queryObject });

                if (references.Total == 1)
                {
                    id = references.Items[0].Id;

                    resolvedReferences[key] = id;

                    cache[(schema, queryJson)] = id;
                }
                else if (references.Total > 1)
                {
                    log.WriteLine("Skipping content {0} / {1} because reference {2} is not unique.", content.File, content.Ref, key);

                    Contents.Remove(content);
                    return;
                }
                else
                {
                    log.WriteLine("Skipping content {0} / {1} because reference {2} cannot be resolved.", content.File, content.Ref, key);

                    Contents.Remove(content);
                    return;
                }
            }

            UpdateData(content.Data, resolvedReferences);
        }

        private void UpdateData(JToken data, Dictionary<string, Guid> resolvedReferences)
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
    }
}
