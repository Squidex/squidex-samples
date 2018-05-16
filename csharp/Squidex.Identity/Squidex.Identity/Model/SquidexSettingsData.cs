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
    public sealed class SquidexSettingsData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string SiteName { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string FooterText { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string PrivacyUrl { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string LegalUrl { get; set; }
    }
}
