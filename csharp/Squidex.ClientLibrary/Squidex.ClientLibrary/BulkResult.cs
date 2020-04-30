// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Squidex.ClientLibrary.Management;

namespace Squidex.ClientLibrary
{
    public sealed class BulkResult
    {
        public Guid ContentId { get; set; }

        public ErrorDto Error { get; set; }
    }
}
