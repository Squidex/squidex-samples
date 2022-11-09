// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel.DataAnnotations;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.App;

internal sealed class AppModel
{
    [Required]
    public Dictionary<string, AppClientModel> Clients { get; set; }

    [Required]
    public Dictionary<string, AppRoleModel> Roles { get; set; }

    [Required]
    public Dictionary<string, UpdateLanguageDto> Languages { get; set; }

    [Required]
    public Dictionary<string, AppContributorModel> Contributors { get; set; }

    public AssetScriptsModel? AssetScripts { get; set; }
}
