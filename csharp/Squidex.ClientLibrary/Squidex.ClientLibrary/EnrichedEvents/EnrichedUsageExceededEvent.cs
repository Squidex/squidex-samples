// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.EnrichedEvents
{
    /// <summary>
    /// Usage Exceeded Event.
    /// </summary>
    public sealed class EnrichedUsageExceededEvent : EnrichedEvent
    {
        /// <summary>
        /// Current calls.
        /// </summary>
        public long CallsCurrent { get; set; }

        /// <summary>
        /// Calls limit.
        /// </summary>
        public long CallsLimit { get; set; }
    }
}
