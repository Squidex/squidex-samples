// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

namespace Squidex.ClientLibrary
{
    public sealed class BulkUpdate
    {
        /// <summary>
        /// The contents to update or insert.
        /// </summary>
        public List<BulkUpdateJob> Jobs { get; set; } = new List<BulkUpdateJob>();

        /// <summary>
        /// True to automatically publish the content.
        /// </summary>
        public bool Publish { get; set; }

        /// <summary>
        /// True to turn off scripting for faster inserts. Default: true.
        /// </summary>
        public bool DoNotScript { get; set; } = true;

        /// <summary>
        /// True to check referrers of this content.
        /// </summary>
        public bool CheckReferrers { get; set; }

        /// <summary>
        /// True to turn off costly validation: Unique checks, asset checks and reference checks. Default: true.
        /// </summary>
        public bool OptimizeValidation { get; set; } = true;
    }
}
