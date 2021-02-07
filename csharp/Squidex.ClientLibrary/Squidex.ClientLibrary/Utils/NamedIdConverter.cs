// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Newtonsoft.Json;

namespace Squidex.ClientLibrary.Utils
{
    /// <summary>
    /// Convert comma separated string to NamedId
    /// Example of input: "00000000-0000-0000-0000-000000000000,name".
    /// </summary>
    public class NamedIdConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NamedId);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var s = ((string)reader.Value).Split(',');

            return new NamedId
            {
                Id = s[0],
                Name = s[1]
            };
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var namedId = (NamedId)value;

            serializer.Serialize(writer, $"{namedId.Id},{namedId.Name}");
        }
    }
}
