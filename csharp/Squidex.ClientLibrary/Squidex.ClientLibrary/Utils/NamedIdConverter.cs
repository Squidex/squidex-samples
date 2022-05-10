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
    /// Convert comma separated string to NamedId.
    /// </summary>
    /// <remarks>
    /// Example of input: "00000000-0000-0000-0000-000000000000,name".
    /// </remarks>
    public class NamedIdConverter : JsonConverter<NamedId>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, NamedId? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, $"{value.Id},{value.Name}");
        }

        /// <inheritdoc />
        public override NamedId? ReadJson(JsonReader reader, Type objectType, NamedId? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var s = ((string)reader.Value!).Split(',');

            return new NamedId { Id = s[0], Name = s[1] };
        }
    }
}
