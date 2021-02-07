// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.EnrichedEvents
{
    /// <summary>
    /// Event on a schema.
    /// </summary>
    public sealed class EnrichedSchemaEvent : EnrichedSchemaEventBase, IEnrichedEntityEvent
    {
        /// <summary>
        /// Type of the event.
        /// </summary>
        public EnrichedSchemaEventType Type { get; set; }

        /// <summary>
        /// Schema id.
        /// </summary>
        public string Id { get; set; }
    }
}
