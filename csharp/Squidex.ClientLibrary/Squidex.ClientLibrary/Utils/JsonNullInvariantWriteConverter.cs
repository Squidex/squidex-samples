// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.ClientLibrary.Utils
{
    internal sealed class JsonNullInvariantWriteConverter<T> : JsonNullConverter<T>
    {
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, JsonNull<T> value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("iv");

            serializer.Serialize(writer, value.Value, typeof(T));

            writer.WriteEndObject();
        }
    }
}
