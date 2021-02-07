// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.EnrichedEvents
{
    /// <summary>
    /// Type of events on an asset.
    /// </summary>
    public enum EnrichedAssetEventType
    {
        /// <summary>
        /// Asset Created.
        /// </summary>
        Created,

        /// <summary>
        /// Asset Deleted.
        /// </summary>
        Deleted,

        /// <summary>
        /// Asset Annotated.
        /// </summary>
        Annotated,

        /// <summary>
        /// Asset Updated.
        /// </summary>
        Updated
    }
}
