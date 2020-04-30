﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary
{
    public class BulkUpdateJob
    {
        public object Data { get; set; }

        public object Query { get; set; }

        public Guid? Id { get; set; }

        public string Status { get; set; }

        public BulkUpdateType Type { get; set; }
    }
}