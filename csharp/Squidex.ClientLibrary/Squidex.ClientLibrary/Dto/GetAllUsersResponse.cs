// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Squidex.ClientLibrary.Dto
{
    public class GetAllUsersResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public bool IsLocked { get; set; }
        public string[] Permissions { get; set; }
    }
}
