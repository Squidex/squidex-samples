// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Squidex.Identity.Model
{
    public sealed class RoleStore : IRoleStore<RoleEntity>
    {
        public void Dispose()
        {
        }

        public Task<RoleEntity> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RoleEntity { Name = roleId, NormalizedName = roleId });
        }

        public Task<RoleEntity> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RoleEntity { Name = normalizedRoleName, NormalizedName = normalizedRoleName });
        }

        public Task<IdentityResult> CreateAsync(RoleEntity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(RoleEntity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(RoleEntity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetNormalizedRoleNameAsync(RoleEntity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(RoleEntity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task<string> GetRoleNameAsync(RoleEntity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(RoleEntity role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(RoleEntity role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;

            return Task.CompletedTask;
        }
    }
}
