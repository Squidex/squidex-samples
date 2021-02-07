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
    /// Abstract class for events on a schema.
    /// </summary>
    public abstract class EnrichedSchemaEventBase : EnrichedUserEventBase
    {
        /// <summary>
        /// Schema changed.
        /// </summary>
        [JsonConverter(typeof(NamedIdConverter))]
        [JsonProperty("schemaId")]
        public NamedId Schema { get; set; }
    }
}
