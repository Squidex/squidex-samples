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
    public sealed class SettingsData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string SiteName { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string FooterText { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string PrivacyUrl { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string LegalUrl { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string BootstrapUrl { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string StylesUrl { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string EmailConfirmationText { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string EmailConfirmationSubject { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string EmailPasswordResetText { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string EmailPasswordResetSubject { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string SmtpFrom { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string SmtpServer { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string SmtpUsername { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string SmtpPassword { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string[] Logo { get; set; }
    }
}
