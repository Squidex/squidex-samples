// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Squidex.ClientLibrary.Utils
{
    public sealed class UTCIsoDateTimeConverter : IsoDateTimeConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime dateTime && dateTime.Kind != DateTimeKind.Utc)
            {
                base.WriteJson(writer, dateTime.ToUniversalTime(), serializer);
            }
            else if (value is DateTimeOffset dateTimeOffset)
            {
                base.WriteJson(writer, dateTimeOffset.UtcDateTime, serializer);
            }
            else
            {
                base.WriteJson(writer, value, serializer);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTimeOffset);
        }
    }
}
