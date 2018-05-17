// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Squidex.ClientLibrary;

namespace Squidex.Identity.Model
{
    public sealed class UserStore :
        IUserPasswordStore<UserEntity>,
        IUserRoleStore<UserEntity>,
        IUserLoginStore<UserEntity>,
        IUserSecurityStampStore<UserEntity>,
        IUserEmailStore<UserEntity>,
        IUserClaimStore<UserEntity>,
        IUserPhoneNumberStore<UserEntity>,
        IUserLockoutStore<UserEntity>,
        IUserAuthenticationTokenStore<UserEntity>
    {
        private readonly SquidexClient<UserEntity, UserData> apiClient;

        public UserStore(SquidexClientManager clientManager)
        {
            apiClient = clientManager.GetClient<UserEntity, UserData>("users");
        }

        public void Dispose()
        {
        }

        public Task<IList<UserEntity>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<IList<UserEntity>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<UserEntity> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return apiClient.GetAsync(userId);
        }

        public async Task<UserEntity> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var result = await apiClient.GetAsync(filter: $"data/normalizedEmail/iv eq '{normalizedEmail}'");

            return result.Items.SingleOrDefault();
        }

        public async Task<UserEntity> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = await apiClient.GetAsync(filter: $"data/normalizedUserName/iv eq '{normalizedUserName}'");

            return result.Items.SingleOrDefault();
        }

        public async Task<UserEntity> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var result = await apiClient.GetAsync(filter: $"data/loginKeys/iv eq '{UserData.LoginKey(loginProvider, providerKey)}'");

            return result.Items.SingleOrDefault();
        }

        public async Task<IdentityResult> CreateAsync(UserEntity user, CancellationToken cancellationToken)
        {
            var result = await apiClient.CreateAsync(user.Data, true);

            user.Id = result.Id;

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(UserEntity user, CancellationToken cancellationToken)
        {
            await apiClient.UpdateAsync(user);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(UserEntity user, CancellationToken cancellationToken)
        {
            await apiClient.DeleteAsync(user);

            return IdentityResult.Success;
        }

        public Task<string> GetUserIdAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.UserName);
        }

        public Task<string> GetNormalizedUserNameAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.PasswordHash);
        }

        public Task<IList<string>> GetRolesAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.GetRoles());
        }

        public Task<bool> IsInRoleAsync(UserEntity user, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.Roles.Contains(roleName));
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.GetLogins());
        }

        public Task<string> GetSecurityStampAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.SecurityStamp);
        }

        public Task<string> GetEmailAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.NormalizedEmail);
        }

        public Task<IList<Claim>> GetClaimsAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.GetClaims());
        }

        public Task<string> GetPhoneNumberAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.PhoneNumberConfirmed);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.LockoutEndDateUtc);
        }

        public Task<int> GetAccessFailedCountAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.LockoutEnabled);
        }

        public Task<string> GetTokenAsync(UserEntity user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.GetToken(loginProvider, name));
        }

        public Task<bool> HasPasswordAsync(UserEntity user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.Data.PasswordHash));
        }

        public Task SetUserNameAsync(UserEntity user, string userName, CancellationToken cancellationToken)
        {
            user.Data.UserName = userName;

            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(UserEntity user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Data.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(UserEntity user, string passwordHash, CancellationToken cancellationToken)
        {
            user.Data.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task AddToRoleAsync(UserEntity user, string roleName, CancellationToken cancellationToken)
        {
            user.Data.AddRole(roleName);

            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(UserEntity user, string roleName, CancellationToken cancellationToken)
        {
            user.Data.RemoveRole(roleName);

            return Task.CompletedTask;
        }

        public Task AddLoginAsync(UserEntity user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            user.Data.AddLogin(login);

            return Task.CompletedTask;
        }

        public Task RemoveLoginAsync(UserEntity user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            user.Data.RemoveLogin(loginProvider, providerKey);

            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(UserEntity user, string stamp, CancellationToken cancellationToken)
        {
            user.Data.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        public Task SetEmailAsync(UserEntity user, string email, CancellationToken cancellationToken)
        {
            user.Data.Email = email;

            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(UserEntity user, bool confirmed, CancellationToken cancellationToken)
        {
            user.Data.EmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(UserEntity user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.Data.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }

        public Task AddClaimsAsync(UserEntity user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            user.Data.AddClaims(claims);

            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(UserEntity user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            user.Data.ReplaceClaim(claim, newClaim);

            return Task.CompletedTask;
        }

        public Task RemoveClaimsAsync(UserEntity user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            user.Data.RemoveClaims(claims);

            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(UserEntity user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.Data.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(UserEntity user, bool confirmed, CancellationToken cancellationToken)
        {
            user.Data.PhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(UserEntity user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.Data.LockoutEndDateUtc = lockoutEnd?.UtcDateTime;

            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(UserEntity user, CancellationToken cancellationToken)
        {
            user.Data.AccessFailedCount++;

            return Task.FromResult(user.Data.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(UserEntity user, CancellationToken cancellationToken)
        {
            user.Data.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(UserEntity user, bool enabled, CancellationToken cancellationToken)
        {
            user.Data.LockoutEnabled = enabled;

            return Task.CompletedTask;
        }

        public Task SetTokenAsync(UserEntity user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            user.Data.SetToken(loginProvider, name, value);

            return Task.CompletedTask;
        }

        public Task RemoveTokenAsync(UserEntity user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            user.Data.RemoveToken(loginProvider, name);

            return Task.CompletedTask;
        }
    }
}
