// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel.DataAnnotations;

namespace Squidex.CLI.Commands.Implementation.Sync.App;

internal sealed class AppRoleModel
{
    [Required]
    public List<string> Permissions { get; set; }

    public Dictionary<string, object>? Properties { get; set; }
}
