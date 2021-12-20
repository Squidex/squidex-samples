// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public static class Extensions
    {
        public static BulkUpdateJob ToUpsert(this ContentModel model, SchemasDto schemas)
        {
            var id = model.Id;

#pragma warning disable CS0618 // Type or member is obsolete
            var singleton = schemas.Items.Find(x => x.Name == model.Schema && x.IsSingleton);
#pragma warning restore CS0618 // Type or member is obsolete
            if (singleton != null)
            {
                id = singleton.Id;
            }

            return new BulkUpdateJob
            {
                Id = id,
                Data = model.Data,
                Schema = model.Schema,
                Status = model.Status,
                Type = BulkUpdateType.Upsert
            };
        }

        public static ContentModel ToModel(this DynamicContent content, string schema)
        {
            return new ContentModel
            {
                Id = content.Id,
                Data = content.Data,
                Status = content.Status,
                Schema = schema
            };
        }

        public static void MapComponents(this DynamicContent content, Dictionary<string, string> map)
        {
            MapComponents(content.Data, map);
        }

        public static void MapComponents(this ContentModel content, Dictionary<string, string> map)
        {
            MapComponents(content.Data, map);
        }

        private static void MapComponents(this DynamicData data, Dictionary<string, string> map)
        {
            static void Map(JToken token, Dictionary<string, string> map)
            {
                if (token is JObject obj)
                {
                    foreach (var value in obj.Values())
                    {
                        Map(value, map);
                    }

                    if (!obj.TryGetValue(Component.Discriminator, StringComparison.Ordinal, out var schemaId))
                    {
                        return;
                    }

                    if (schemaId.Type == JTokenType.String && map.TryGetValue(schemaId!.Value<string>()!, out var target))
                    {
                        obj[Component.Discriminator] = target;
                    }
                }
                else if (token is JArray array)
                {
                    foreach (var value in array)
                    {
                        Map(value, map);
                    }
                }
            }

            foreach (var field in data.Values)
            {
                Map(field, map);
            }
        }

        public static void Clear(this ContentsModel model, string[] languages)
        {
            if (languages?.Length > 0 && model.Contents?.Count > 0)
            {
                var allowedLanguages = languages.ToHashSet();

                var toClear = new List<string>();

                foreach (var content in model.Contents)
                {
                    foreach (var field in content.Data.Values)
                    {
                        foreach (var language in field.Children<JProperty>().Select(x => x.Name))
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
    }
}
