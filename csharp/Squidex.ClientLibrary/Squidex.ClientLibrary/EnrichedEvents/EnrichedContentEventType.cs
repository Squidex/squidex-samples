// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.EnrichedEvents
{
    /// <summary>
    /// Type of event on a content.
    /// </summary>
    public enum EnrichedContentEventType
    {
        /// <summary>
        /// Content Created.
        /// </summary>
        Created,

        /// <summary>
        /// Content Deleted.
        /// </summary>
        Deleted,

        /// <summary>
        /// Content Published.
        /// </summary>
        Published,

        /// <summary>
        /// Content Status Changed.
        /// </summary>
        StatusChanged,

        /// <summary>
        /// Content Updated.
        /// </summary>
        Updated,

        /// <summary>
        /// Content Unpublished.
        /// </summary>
        Unpublished
    }
}
