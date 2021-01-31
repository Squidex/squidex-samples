// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// Default status strings.
    /// </summary>
    public static class Status
    {
        /// <summary>
        /// Content is Archived (soft-delete).
        /// </summary>
        public const string Archived = "Archived";

        /// <summary>
        /// Content is not ready and not available in the API by default.
        /// </summary>
        public const string Draft = "Draft";

        /// <summary>
        /// Content is ready and published.
        /// </summary>
        public const string Published = "Published";
    }
}
