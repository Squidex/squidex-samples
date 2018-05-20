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
    public sealed class ClientData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string AllowedScopes { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string ClientSecrets { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string ClientId { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string ClientName { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string ClientUri { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string[] Logo { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string AllowedCorsOrigins { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string AllowedGrantTypes { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string RedirectUris { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string PostLogoutRedirectUris { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public bool AllowOfflineAccess { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public bool RequireConsent { get; set; }
    }
}
