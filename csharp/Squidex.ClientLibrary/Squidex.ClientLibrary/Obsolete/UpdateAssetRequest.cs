// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Squidex.ClientLibrary
{
    [Obsolete]
    public class UpdateAssetRequest
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
    }
}
