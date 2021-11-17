// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.App
{
    public static class Extensions
    {
        public static AppRoleModel ToModel(this RoleDto role)
        {
            return new AppRoleModel { Permissions = role.Permissions, Properties = role.Properties };
        }

        public static UpdateRoleDto ToUpdate(this AppRoleModel model)
        {
            return new UpdateRoleDto { Permissions = model.Permissions, Properties = model.Properties };
        }

        public static AppClientModel ToModel(this ClientDto client)
        {
            return new AppClientModel { Name = client.Name, Role = client.Role };
        }

        public static UpdateClientDto ToUpdate(this AppClientModel model)
        {
            return new UpdateClientDto { Name = model.Name, Role = model.Role };
        }

        public static UpdateLanguageDto ToModel(this AppLanguageDto language)
        {
            return new UpdateLanguageDto
            {
                Fallback = language.Fallback,
                IsMaster = language.IsMaster,
                IsOptional = language.IsOptional
            };
        }
    }
}
