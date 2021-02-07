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
    /// Convert actor string
    /// Example of input: "subject:123456789".
    /// </summary>
    public class ActorConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Actor);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var s = ((string)reader.Value).Split(':');

            return new Actor
            {
                Id = s[1],
                Type = s[0]
            };
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var actor = (Actor)value;

            serializer.Serialize(writer, $"{actor.Type}:{actor.Id}");
        }
    }
}
