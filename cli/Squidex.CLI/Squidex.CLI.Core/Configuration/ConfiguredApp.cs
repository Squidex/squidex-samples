// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Configuration
{
    public sealed class ConfiguredApp
    {
        public string ServiceUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Name { get; set; }

        public bool IgnoreSelfSigned { get; set; }
    }
}
