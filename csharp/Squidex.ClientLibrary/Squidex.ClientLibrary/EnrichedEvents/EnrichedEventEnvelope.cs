// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.EnrichedEvents
{
    /// <summary>
    /// Envelope which contains the generated events.
    /// </summary>
    public class EnrichedEventEnvelope
    {
        /// <summary>
        /// Type of the event.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// When the event has been generated.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// The payload of the evnet.
        /// </summary>
        public EnrichedEvent Payload { get; set; }

        /// <summary>
        /// Utils to deserialize an Envelope.
        /// </summary>
        /// <param name="json">The string to be deserialized.</param>
        /// <param name="settings">Custom JsonSerializerSettings settings. TypeNameHandling and SerializationBinder will be overwritten.</param>
        /// <returns>
        /// The enriched event.
        /// </returns>
        [Obsolete("Use FromJson instead.")]
        public static EnrichedEventEnvelope DeserializeEnvelope(string json, JsonSerializerSettings? settings = null)
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings();
            }

            settings.SerializationBinder = new EnrichedEventSerializationBinder();
            settings.TypeNameHandling = TypeNameHandling.Auto;

            return JsonConvert.DeserializeObject<EnrichedEventEnvelope>(json, settings)!;
        }

        /// <summary>
        /// Utils to deserialize an Envelope.
        /// </summary>
        /// <param name="json">The string to be deserialized.</param>
        /// <returns>
        /// The enriched event.
        /// </returns>
        public static EnrichedEventEnvelope FromJson(string json)
        {
            return HttpClientExtensions.FromJsonWithTypes<EnrichedEventEnvelope>(json);
        }
    }
}
