// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Squidex.ClientLibrary;

namespace Squidex.Identity.Model.Authentication
{
    public sealed class AuthenticationSchemeData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public AuthenticationSchemeType Provider { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string ClientId { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string ClientSecret { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string[] Scopes { get; set; }
    }
}
