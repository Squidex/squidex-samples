// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.ClientLibrary;

/// <summary>
/// A JSON converter to for invariant fields to convert nested invariant values to flat values.
/// </summary>
/// <seealso cref="JsonConverter" />
public sealed class InvariantConverter : JsonConverter
{
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("iv");

        serializer.Serialize(writer, value);

        writer.WriteEndObject();
    }

    /// <inheritdoc/>
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        reader.Read();

        if (reader.TokenType == JsonToken.EndObject)
        {
            // empty object
            return null;
        }
        else if (reader.TokenType != JsonToken.PropertyName || !string.Equals(reader.Value?.ToString(), "iv", StringComparison.OrdinalIgnoreCase))
        {
            throw new JsonSerializationException("Property must have a invariant language property.");
        }

        reader.Read();

        var result = serializer.Deserialize(reader, objectType);

        reader.Read();

        return result;
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return false;
    }
}
