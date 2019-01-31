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
    public class CreateAppRequest
    {
        public string Name { get; set; }
        public string Template { get; set; }
    }
}
