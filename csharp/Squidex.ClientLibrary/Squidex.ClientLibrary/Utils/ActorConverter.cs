// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.ClientLibrary.Utils
{
    /// <summary>
    /// Convert actor string
    /// Example of input: "subject:123456789".
    /// </summary>
    public class ActorConverter : JsonConverter<Actor>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Actor? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, $"{value.Type}:{value.Id}");
        }

        /// <inheritdoc />
        public override Actor? ReadJson(JsonReader reader, Type objectType, Actor? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var s = ((string)reader.Value!).Split(':');

            return new Actor { Id = s[1], Type = s[0] };
        }
    }
}
