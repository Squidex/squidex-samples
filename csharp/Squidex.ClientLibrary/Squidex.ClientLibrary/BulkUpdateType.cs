// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// The type of the bulk update.
    /// </summary>
    public enum BulkUpdateType
    {
        /// <summary>
        /// Update or create a new content item.
        /// </summary>
        Upsert,

        /// <summary>
        /// Change the status of a content item.
        /// </summary>
        ChangeStatus,

        /// <summary>
        /// Delete a content item.
        /// </summary>
        Delete,

        /// <summary>
        /// Create a content item.
        /// </summary>
        Create,

        /// <summary>
        /// Patch a content item.
        /// </summary>
        Patch,

        /// <summary>
        /// Update a content item.
        /// </summary>
        Update,

        /// <summary>
        /// Validate a content item.
        /// </summary>
        Validate
    }
}