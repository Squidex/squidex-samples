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
    /// Avstract class for events triggered by an Actor.
    /// </summary>
    public abstract class EnrichedUserEventBase : EnrichedEvent
    {
        /// <summary>
        /// Actor who has triggered the event.
        /// </summary>
        [JsonConverter(typeof(ActorConverter))]
        public Actor Actor { get; set; }
    }
}
