// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Newtonsoft.Json;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.EnrichedEvents
{
    /// <summary>
    /// Abstract class for the events sent by the rules.
    /// </summary>
    public abstract class EnrichedEvent
    {
        /// <summary>
        /// Application that generated the event.
        /// </summary>
        [JsonConverter(typeof(NamedIdConverter))]
        [JsonProperty("appId")]
        public NamedId App { get; set; }

        /// <summary>
        /// When the event has been generated.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Name of the event.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Version of the object.
        /// </summary>
        public long Version { get; set; }
    }
}
