// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Squidex.ClientLibrary
{
    public abstract class Resource
    {
        [JsonProperty("_links")]
        public Dictionary<string, ResourceLink> Links { get; } = new Dictionary<string, ResourceLink>();
    }
}
