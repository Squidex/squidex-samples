// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Integration.Localize.Controllers;

public partial class DefaultController : ControllerBase
{
    public override async Task<CacheResponse> Cache(
        CancellationToken cancellationToken = default)
    {
        var clientManager = BuildClientManager();

        var response = new CacheResponse
        {
            Items = new List<UniqueItemIdentifier>()
        };

        var schemas =
            await clientManager.CreateSchemasClient().GetSchemasAsync(clientManager.App,
                cancellationToken);

        foreach (var schema in schemas.Items)
        {
            var localizedFields = schema.Fields.Where(x => x.Partitioning == "language" && x.Properties is StringFieldPropertiesDto).ToList();

            // If the schema has no localized fields, just skip it.
            if (localizedFields.Count == 0)
            {
                continue;
            }

            var contents = clientManager.CreateDynamicContentsClient(schema.Name);

            await contents.GetAllAsync(content =>
            {
                foreach (var field in localizedFields)
                {
                    response.Items.Add(BuildItem(new UniqueItemIdentifier(), content, field.Name));
                }

                return Task.CompletedTask;
            }, 200, QueryContext.Default.Unpublished(), cancellationToken);
        }

        return response;
    }

    public override async Task<CacheItemsResponse> Items([FromBody] CacheItemsRequest body,
        CancellationToken cancellationToken = default)
    {
        var clientManager = BuildClientManager();

        var response = new CacheItemsResponse
        {
            Items = new List<CacheItem>()
        };

        var languages =
            await clientManager.CreateAppsClient().GetLanguagesAsync(clientManager.App,
                cancellationToken);

        var masterLanguage = languages.Items.Find(x => x.IsMaster)?.Iso2Code ?? "en";

        var schemas =
            await clientManager.CreateSchemasClient().GetSchemasAsync(clientManager.App,
                cancellationToken);

        var schemaDefinitions =
            schemas.Items.ToDictionary(
                s => s.Name,
                s =>
                {
                    var listFields = s.Fields.Where(x => s.FieldsInLists?.Contains(x.Name) == true).ToList();

                    if (listFields.Count == 0)
                    {
                        listFields = s.Fields.Take(1).ToList();
                    }

                    return listFields;
                });

        var fieldsById = body.Items.Select(x => new
        {
            ContentId = x.Metadata[MetaFields.ContentId],
            ContentField = x.Metadata[MetaFields.ContentField],
        }).ToLookup(x => x.ContentId);

        var contentClient = clientManager.CreateDynamicContentsClient("none");
        var contentIds = body.Items.Select(x => x.Metadata[MetaFields.ContentId]).Distinct().ToHashSet();

        foreach (var batch in contentIds.Batch(200))
        {
            var contents =
                await contentClient.GetAsync(batch.ToHashSet(), QueryContext.Default.Unpublished(),
                    cancellationToken);

            foreach (var content in contents.Items)
            {
                if (!schemaDefinitions.TryGetValue(content.SchemaName, out var listFields))
                {
                    continue;
                }

                var title = TitleBuilder.BuildTitle(content, listFields, masterLanguage);

                foreach (var field in fieldsById[content.Id].Select(x => x.ContentField))
                {
                    var item = new CacheItem
                    {
                        Title = field,
                        Fields = new Dictionary<string, string>
                        {
                            [MetaFields.SchemaName] = content.SchemaName,
                            [MetaFields.ContentId] = content.Id,
                            [MetaFields.DateCreated] = content.Created.ToString(CultureInfo.InvariantCulture),
                            [MetaFields.DateUpdated] = content.LastModified.ToString(CultureInfo.InvariantCulture)
                        },
                        GroupTitle = title
                    };

                    response.Items.Add(BuildItem(item, content, field));
                }
            }
        }

        return response;
    }

    public override async Task<TranslateResponse> Translate([FromBody] TranslateRequest body,
        CancellationToken cancellationToken = default)
    {
        var clientManager = BuildClientManager();

        var response = new TranslateResponse
        {
            Items = new List<ContentItem>()
        };

        var fieldsById = body.Items.Select(x => new
        {
            ContentId = x.Metadata[MetaFields.ContentId],
            ContentField = x.Metadata[MetaFields.ContentField],
        }).ToLookup(x => x.ContentId);

        var contentClient = clientManager.CreateDynamicContentsClient("none");
        var contentIds = body.Items.Select(x => x.Metadata[MetaFields.ContentId]).Distinct().ToHashSet();

        foreach (var batch in contentIds.Batch(200))
        {
            var contents =
                await contentClient.GetAsync(batch.ToHashSet(), QueryContext.Default.Unpublished(),
                    cancellationToken);

            foreach (var content in contents.Items)
            {
                foreach (var field in fieldsById[content.Id].Select(x => x.ContentField))
                {
                    var translations = new Dictionary<string, string>();

                    if (content.Data.TryGetValue(field, out var temp) && temp is JObject value)
                    {
                        foreach (var (key, fieldValue) in value)
                        {
                            if (fieldValue is not null && fieldValue.Type is not JTokenType.Array or JTokenType.Object)
                            {
                                translations[key] = fieldValue.ToString();
                            }
                        }
                    }

                    var item = new ContentItem
                    {
                        Translations = translations
                    };

                    response.Items.Add(BuildItem(item, content, field));
                }
            }
        }

        return response;
    }

    private static T BuildItem<T>(T identifier, DynamicContent content, string field) where T : UniqueItemIdentifier
    {
        identifier.GroupId = content.Id;
        identifier.UniqueId = $"{content.Id}_{field}";
        identifier.Metadata = new UniqueIdMetadata
        {
            [MetaFields.ContentId] = content.Id,
            [MetaFields.ContentField] = field,
            [MetaFields.SchemaName] = content.SchemaName
        };

        return identifier;
    }
}
