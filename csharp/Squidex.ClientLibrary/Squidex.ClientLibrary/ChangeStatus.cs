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
    /// The request to change a status.
    /// </summary>
    public sealed class ChangeStatus
    {
        /// <summary>
        /// The new status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The due time.
        /// </summary>
        public DateTime? DueTime { get; set; }

        /// <summary>
        /// True to check referrers of this content.
        /// </summary>
        public bool CheckReferrers { get; set; }
    }
}
