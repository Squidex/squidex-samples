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
        public List<BulkUpdateJob> Jobs { get; set; } = new List<BulkUpdateJob>();

        public bool Publish { get; set; }

        public bool DoNotScript { get; set; } = true;

        public bool OptimizeValidation { get; set; } = true;
    }
}
