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
    public class CreateClientResponse
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
