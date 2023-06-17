// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.ClientLibrary.Utils;

internal sealed class ListFallbackConverter<T, TItem> : JsonConverter<T>
{
    private readonly Func<List<TItem>, T> factory;

    internal ListFallbackConverter(Func<List<TItem>, T> factory)
    {
        this.factory = factory;
    }

    public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartArray)
        {
            var items = ListFallbackConverter.DefaultSerializer.Deserialize<List<TItem>>(reader)!;

            return factory(items);
        }

        return ListFallbackConverter.DefaultSerializer.Deserialize<T?>(reader);
    }

    public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
    {
        ListFallbackConverter.DefaultSerializer.Serialize(writer, value);
    }
}
