// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace Squidex.ClientLibrary.Transfer
{
    public class CreateAppResponse
    {
        public string Id { get; set; }
        public string[] Permissions { get; set; }
        public long Version { get; set; }
        public string PlanName { get; set; }
    }
}
