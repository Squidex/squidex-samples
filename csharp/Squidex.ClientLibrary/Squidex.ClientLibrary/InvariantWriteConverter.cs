// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Newtonsoft.Json;

namespace Squidex.ClientLibrary
{
    public sealed class InvariantWriteConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("iv");

            serializer.Serialize(writer, value);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);

            reader.Read();

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }
}
