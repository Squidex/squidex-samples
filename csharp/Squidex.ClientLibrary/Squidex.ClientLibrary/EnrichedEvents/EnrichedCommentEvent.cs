// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary.EnrichedEvents
{
    /// <summary>
    /// Event generated from a comment.
    /// </summary>
    public sealed class EnrichedCommentEvent : EnrichedUserEventBase
    {
        /// <summary>
        /// Comment's text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Url of the content commented.
        /// </summary>
        public Uri Url { get; set; }
    }
}
