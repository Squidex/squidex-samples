// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Squidex.ClientLibrary.Utils
{
    /// <summary>
    /// A Json converter to serialize <see cref="DateTime"/> and <see cref="DateTimeOffset"/>
    /// values to ISO-8601 compliant strings (yyy-MM-dd'T'HH:mm:ss).
    /// </summary>
    public sealed class UTCIsoDateTimeConverter : IsoDateTimeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UTCIsoDateTimeConverter"/> class.
        /// </summary>
        public UTCIsoDateTimeConverter()
        {
            DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
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

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTimeOffset);
        }
    }
}
