// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel.DataAnnotations;

namespace Squidex.CLI.Commands.Implementation.Sync.App
{
    public sealed class AppRoleSetting
    {
        [Required]
        public string[] Permissions { get; set; }
    }
}
