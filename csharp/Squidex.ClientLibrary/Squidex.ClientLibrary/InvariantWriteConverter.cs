// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.ClientLibrary;

/// <summary>
/// A JSON converter for invariant fields to convert nested invariant values to flat values when serializing objects to JSON.
/// </summary>
/// <seealso cref="JsonConverter" />
public sealed class InvariantWriteConverter : JsonConverter
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
        return serializer.Deserialize(reader, objectType);
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return false;
    }
}
