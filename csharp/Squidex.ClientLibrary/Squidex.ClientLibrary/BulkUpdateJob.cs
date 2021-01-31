// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// Represents an job of a bulk operation.
    /// </summary>
    public class BulkUpdateJob
    {
        /// <summary>
        /// The data of the content when type is set to 'Upsert', 'Create', 'Update' or 'Patch'.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// An optional query to identify the content to update.
        /// </summary>
        public object Query { get; set; }

        /// <summary>
        /// An optional id of the content to update.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The new status when the type is set to 'ChangeStatus'.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The optional schema id or name.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// The due time when the type is set to 'ChangeStatus'.
        /// </summary>
        public DateTime? DueTime { get; set; }

        /// <summary>
        /// The update type.
        /// </summary>
        public BulkUpdateType Type { get; set; }

        /// <summary>
        /// The number of expected items. Set it to a higher number to update multiple items when a query is defined.
        /// </summary>
        public long ExpectedCount { get; set; } = 1;

        /// <summary>
        /// The expected version.
        /// </summary>
        public long ExpectedVersion { get; set; } = -2;
    }
}