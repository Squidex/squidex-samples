// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json.Linq;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public static class Extensions
    {
        public static BulkUpdateJob ToUpsert(this ContentModel model, SchemasDto schemas, bool patch)
        {
            var result = SimpleMapper.Map(model, new BulkUpdateJob { Patch = patch });

#pragma warning disable CS0618 // Type or member is obsolete
            var singleton = schemas.Items.Find(x => x.Name == model.Schema && (x.IsSingleton || x.Type == SchemaType.Singleton));
#pragma warning restore CS0618 // Type or member is obsolete
            if (singleton != null)
            {
                result.Id = singleton.Id;
            }

            result.Data = model.Data;

            return result;
        }

        public static ContentModel ToModel(this DynamicContent content, string schema)
        {
            var result = SimpleMapper.Map(content, new ContentModel());

            result.Schema = schema;

            return result;
        }

        public static void MapComponents(this DynamicContent content, Dictionary<string, string> map)
        {
            MapComponents(content.Data, map);
        }

        public static void MapComponents(this ContentModel model, Dictionary<string, string> map)
        {
            MapComponents(model.Data, map);
        }

        public sealed class Mapper
        {
            private readonly string url;
            private readonly string pathToAssets;
            private readonly string pathToContent;
            private readonly HashSet<string>? languages;
            private string? sourceUrl;
            private string? sourceApp;
            private string? sourcePathToAssets;
            private string? sourcePathToContent;

            public Mapper(string url, string app, string[]? languages)
            {
                this.url = url.TrimEnd('/');

                pathToAssets = $"api/assets/{app}/";
                pathToContent = $"api/content/{app}/";

                this.languages = languages?.ToHashSet();
            }

            public void Map(ContentsModel model)
            {
                if (model.Contents == null || model.Contents.Count == 0)
                {
                    return;
                }

                sourceUrl = model.SourceUrl?.TrimEnd('/');

                if (model.SourceApp != sourceApp)
                {
                    sourceApp = model.SourceApp;

                    if (sourceApp != null)
                    {
                        sourcePathToAssets = $"api/assets/{sourceApp}/";
                        sourcePathToContent = $"api/content/{sourceApp}/";
                    }
                }

                foreach (var content in model.Contents)
                {
                    ClearLanguages(content);

                    MapStrings(content);
                }
            }

            private void MapStrings(ContentModel content)
            {
                if (string.IsNullOrWhiteSpace(sourceUrl))
                {
                    return;
                }

                string? ReplaceUrl(string? value)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return value;
                    }

                    if (sourceUrl != null)
                    {
                        value = value?.Replace(sourceUrl, url, StringComparison.OrdinalIgnoreCase);
                    }

                    if (sourcePathToAssets != null)
                    {
                        value = value?.Replace(sourcePathToAssets, pathToAssets, StringComparison.OrdinalIgnoreCase);
                    }

                    if (sourcePathToContent != null)
                    {
                        value = value?.Replace(sourcePathToContent, pathToContent, StringComparison.OrdinalIgnoreCase);
                    }

                    return value;
                }

                void Map(JToken? value)
                {
                    switch (value)
                    {
                        case JArray array:
                            {
                                for (var i = 0; i < array.Count; i++)
                                {
                                    var item = array[i];

                                    if (item.Type == JTokenType.String)
                                    {
                                        array[i] = ReplaceUrl(item?.ToString());
                                    }
                                    else
                                    {
                                        Map(item);
                                    }
                                }

                                break;
                            }

                        case IDictionary<string, JToken?> obj:
                            {
                                foreach (var (key, item) in obj.ToList())
                                {
                                    if (item?.Type == JTokenType.String)
                                    {
                                        obj[key] = ReplaceUrl(item?.ToString());
                                    }
                                    else
                                    {
                                        Map(item);
                                    }
                                }

                                break;
                            }
                    }
                }

                foreach (var item in content.Data.Values)
                {
                    Map(item);
                }
            }

            private void ClearLanguages(ContentModel model)
            {
                if (languages == null || languages.Count > 0)
                {
                    return;
                }

                foreach (var field in model.Data.Values.OfType<JObject>())
                {
                    foreach (var property in field.Properties().ToList())
                    {
                        if (property.Name != "iv" && !languages.Contains(property.Name))
                        {
                            field.Remove(property.Name);
                        }
                    }
                }
            }
        }

        public static void MapComponents(this DynamicData data, Dictionary<string, string> map)
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
    }
}
