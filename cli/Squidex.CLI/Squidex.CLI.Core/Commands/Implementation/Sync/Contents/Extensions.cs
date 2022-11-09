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

namespace Squidex.CLI.Commands.Implementation.Sync.Contents;

internal static class Extensions
{
    public static BulkUpdateJob ToUpsert(this ContentModel model, SchemasDto schemas, ContentAction action)
    {
        var result = SimpleMapper.Map(model, new BulkUpdateJob());

#pragma warning disable CS0618 // Type or member is obsolete
        var singleton = schemas.Items.Find(x => x.Name == model.Schema && (x.IsSingleton || x.Type == SchemaType.Singleton));
#pragma warning restore CS0618 // Type or member is obsolete
        if (singleton != null)
        {
            result.Id = singleton.Id;
        }

        switch (action)
        {
            case ContentAction.UpsertPatch:
                result.Patch = true;
                break;
            case ContentAction.Create:
                result.Type = BulkUpdateType.Create;
                break;
            case ContentAction.Update:
                result.Type = BulkUpdateType.Update;
                break;
            case ContentAction.Patch:
                result.Type = BulkUpdateType.Patch;
                break;
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
        private readonly string urlToAssets;
        private readonly string urlToContent;
        private readonly HashSet<string>? languages;
        private string? sourceApp;
        private string? sourceUrl;
        private string? sourceUrlToAssets;
        private string? sourceUrlToContent;

        public Mapper(string url, string app, string[]? languages)
        {
            this.url = url.TrimEnd('/');

            urlToAssets = $"{this.url}/api/assets/{app}/";
            urlToContent = $"{this.url}/api/content/{app}/";

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
                    sourceUrlToAssets = $"{sourceUrl}/api/assets/{sourceApp}/";
                    sourceUrlToContent = $"{sourceUrl}/api/content/{sourceApp}/";
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

                if (sourceUrlToAssets != null)
                {
                    value = value?.Replace(sourceUrlToAssets, urlToAssets, StringComparison.OrdinalIgnoreCase);
                }

                if (sourceUrlToContent != null)
                {
                    value = value?.Replace(sourceUrlToContent, urlToContent, StringComparison.OrdinalIgnoreCase);
                }

                if (sourceUrl != null)
                {
                    value = value?.Replace(sourceUrl, url, StringComparison.OrdinalIgnoreCase);
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
