// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Squidex.ClientLibrary;

namespace Squidex.Identity.Model
{
    public sealed class ResourceData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string Name { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string DisplayName { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string Description { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public bool Required { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string UserClaims { get; set; }
    }
}
