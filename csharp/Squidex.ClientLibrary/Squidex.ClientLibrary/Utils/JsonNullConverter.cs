// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.ClientLibrary.Utils;

/// <summary>
/// A JSON converter for <see cref="JsonNull{T}"/> instances.
/// </summary>
/// <typeparam name="T">The wrapped type.</typeparam>
public class JsonNullConverter<T> : JsonConverter<JsonNull<T>>
{
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, JsonNull<T> value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.Value, typeof(T));
    }

    /// <inheritdoc/>
    public override JsonNull<T> ReadJson(JsonReader reader, Type objectType, JsonNull<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var value = serializer.Deserialize<T>(reader);

        return new JsonNull<T>(value!);
    }
}
