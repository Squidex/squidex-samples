// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.EnrichedEvents
{
    /// <summary>
    /// Schema event types.
    /// </summary>
    public enum EnrichedSchemaEventType
    {
        /// <summary>
        /// Schema created.
        /// </summary>
        Created,

        /// <summary>
        /// Schema Deleted.
        /// </summary>
        Deleted,

        /// <summary>
        /// Schema Published.
        /// </summary>
        Published,

        /// <summary>
        /// Schema Unpublished.
        /// </summary>
        Unpublished,

        /// <summary>
        /// Schema updated.
        /// </summary>
        Updated
    }
}
