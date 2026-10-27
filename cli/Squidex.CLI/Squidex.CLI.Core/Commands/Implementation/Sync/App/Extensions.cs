// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.App;

internal static class Extensions
{
    public static AppRoleModel ToModel(this RoleDto role)
    {
        return SimpleMapper.Map(role, new AppRoleModel(), CultureInfo.InvariantCulture);
    }

    public static UpdateRoleDto ToUpdate(this AppRoleModel model)
    {
        return SimpleMapper.Map(model, new UpdateRoleDto(), CultureInfo.InvariantCulture);
    }

    public static AppClientModel ToModel(this ClientDto client)
    {
        return SimpleMapper.Map(client, new AppClientModel(), CultureInfo.InvariantCulture);
    }

    public static UpdateClientDto ToUpdate(this AppClientModel model)
    {
        return SimpleMapper.Map(model, new UpdateClientDto(), CultureInfo.InvariantCulture);
    }

    public static UpdateLanguageDto ToModel(this AppLanguageDto language)
    {
        return SimpleMapper.Map(language, new UpdateLanguageDto(), CultureInfo.InvariantCulture);
    }

    public static UpdateAssetScriptsDto ToUpdate(this AssetScriptsModel model)
    {
        return SimpleMapper.Map(model, new UpdateAssetScriptsDto(), CultureInfo.InvariantCulture);
    }

    public static AssetScriptsModel ToModel(this AssetScriptsDto model)
    {
        return SimpleMapper.Map(model, new AssetScriptsModel(), CultureInfo.InvariantCulture);
    }
}
