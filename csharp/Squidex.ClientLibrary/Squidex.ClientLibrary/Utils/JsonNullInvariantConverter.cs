// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.ClientLibrary.Utils
{
    internal sealed class JsonNullInvariantConverter<T> : JsonConverter<JsonNull<T>>
    {
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, JsonNull<T> value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("iv");

            serializer.Serialize(writer, value.Value, typeof(T));

            writer.WriteEndObject();
        }

        /// <inheritdoc/>
        public override JsonNull<T> ReadJson(JsonReader reader, Type objectType, JsonNull<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return default;
            }

            reader.Read();

            if (reader.TokenType == JsonToken.EndObject)
            {
                // empty object
                return default;
            }
            else if (reader.TokenType != JsonToken.PropertyName || !string.Equals(reader.Value?.ToString(), "iv", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonSerializationException("Property must have a invariant language property.");
            }

            reader.Read();

            var result = serializer.Deserialize<JsonNull<T>>(reader)!;

            reader.Read();

            return result;
        }
    }
}
