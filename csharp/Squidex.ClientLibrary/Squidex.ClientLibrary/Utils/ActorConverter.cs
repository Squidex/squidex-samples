// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            var s = (reader.Value as string).Split(':');

            return new Actor()
            {
                Id = s[1],
                Type = s[0]
            };
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var actor = value as Actor;
            serializer.Serialize(writer, $"{actor.Type}:{actor.Id}");
        }
    }
}
