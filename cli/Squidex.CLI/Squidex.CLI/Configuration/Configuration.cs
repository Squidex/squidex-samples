// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

namespace Squidex.CLI.Configuration
{
    public sealed class Configuration
    {
        public Dictionary<string, ConfiguredApp> Apps { get; } = new Dictionary<string, ConfiguredApp>();

        public string CurrentApp { get; set; }
    }
}
