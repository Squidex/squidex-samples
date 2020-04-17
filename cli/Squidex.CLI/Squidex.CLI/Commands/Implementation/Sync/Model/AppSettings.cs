// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Model
{
    public sealed class AppSettings
    {
        [Required]
        public Dictionary<string, AppClientSetting> Clients { get; set; }

        [Required]
        public Dictionary<string, AppRoleSetting> Roles { get; set; }

        [Required]
        public Dictionary<string, UpdateLanguageDto> Languages { get; set; }

        [Required]
        public Dictionary<string, string> Contributors { get; set; }
    }
}
