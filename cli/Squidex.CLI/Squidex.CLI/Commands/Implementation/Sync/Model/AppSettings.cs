// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Squidex.CLI.Commands.Implementation.Sync.Model
{
    public sealed class AppSettings
    {
        [Required]
        public Dictionary<string, ClientSetting> Clients { get; set; }

        [Required]
        public Dictionary<string, ContributorSetting> Contributors { get; set; }
    }
}
