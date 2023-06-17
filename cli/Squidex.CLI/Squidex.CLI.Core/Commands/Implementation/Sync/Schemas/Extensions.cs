// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Schemas;

internal static class Extensions
{
    public static CreateSchemaDto ToCreate(this SchemaCreateModel model)
    {
        var result = SimpleMapper.Map(model, new CreateSchemaDto());

        if (model.IsSingleton && model.SchemaType == SchemaType.Default)
        {
            result.Type = SchemaType.Singleton;
        }
        else
        {
            result.Type = model.SchemaType;
        }

        return result;
    }

    public static async Task MapFoldersAsync(this SynchronizeSchemaDto schema, AssetFolderTree folders, bool fromId)
    {
        foreach (var field in schema.Fields ?? Enumerable.Empty<UpsertSchemaFieldDto>())
        {
            await field.MapFoldersAsync(folders, fromId);
        }
    }

    public static async Task MapFoldersAsync(this UpsertSchemaFieldDto field, AssetFolderTree folders, bool fromId)
    {
        await MapFoldersAsync(field.Properties, folders, fromId);

        foreach (var nested in field.Nested ?? Enumerable.Empty<UpsertSchemaNestedFieldDto>())
        {
            await MapFoldersAsync(nested.Properties, folders, fromId);
        }
    }

    private static async Task MapFoldersAsync(FieldPropertiesDto properties, AssetFolderTree folders, bool fromId)
    {
        switch (properties)
        {
            case AssetsFieldPropertiesDto assets:
                assets.FolderId = await MapFoldersAsync(assets.FolderId, folders, fromId);
                break;
            case StringFieldPropertiesDto strings:
                strings.FolderId = await MapFoldersAsync(strings.FolderId, folders, fromId);
                break;
        }
    }

    private static Task<string?> MapFoldersAsync(string? folderId, AssetFolderTree folders, bool fromId)
    {
        if (fromId)
        {
            return folders.GetPathAsync(folderId);
        }
        else
        {
            return folders.GetIdAsync(folderId);
        }
    }

    public static async Task MapReferencesAsync(this SynchronizeSchemaDto schema, Dictionary<string, string> map)
    {
        foreach (var field in schema.Fields ?? Enumerable.Empty<UpsertSchemaFieldDto>())
        {
            await MapReferencesAsync(field, map);
        }
    }

    public static async Task MapReferencesAsync(this UpsertSchemaFieldDto field, Dictionary<string, string> map)
    {
        await MapReferencesAsync(field.Properties, map);

        foreach (var nested in field.Nested ?? Enumerable.Empty<UpsertSchemaNestedFieldDto>())
        {
            await MapReferencesAsync(nested.Properties, map);
        }
    }

    private static async Task MapReferencesAsync(FieldPropertiesDto properties, Dictionary<string, string> map)
    {
        switch (properties)
        {
            case ReferencesFieldPropertiesDto references:
                references.SchemaIds = await MapReferencesAsync(references.SchemaIds, map);
                break;
            case ComponentFieldPropertiesDto component:
                component.SchemaIds = await MapReferencesAsync(component.SchemaIds, map);
                break;
            case ComponentsFieldPropertiesDto components:
                components.SchemaIds = await MapReferencesAsync(components.SchemaIds, map);
                break;
            case StringFieldPropertiesDto @string:
                @string.SchemaIds = await MapReferencesAsync(@string.SchemaIds, map);
                break;
        }
    }

    private static Task<List<string>?> MapReferencesAsync(List<string>? ids, Dictionary<string, string> map)
    {
        if (ids == null || ids.Count == 0)
        {
            return Task.FromResult(ids);
        }

        var result = new List<string>();

        foreach (var id in ids)
        {
            if (map.TryGetValue(id, out var target))
            {
                result.Add(target);
            }
        }

        return Task.FromResult<List<string>?>(result);
    }
}
