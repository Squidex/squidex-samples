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
        IUserPasswordStore<SquidexUser>,
        IUserRoleStore<SquidexUser>,
        IUserLoginStore<SquidexUser>,
        IUserSecurityStampStore<SquidexUser>,
        IUserEmailStore<SquidexUser>,
        IUserClaimStore<SquidexUser>,
        IUserPhoneNumberStore<SquidexUser>,
        IUserLockoutStore<SquidexUser>,
        IUserAuthenticationTokenStore<SquidexUser>
    {
        private readonly SquidexClient<SquidexUser, SquidexUserData> apiClient;

        public UserStore(SquidexClientManager clientManager)
        {
            apiClient = clientManager.GetClient<SquidexUser, SquidexUserData>("users");
        }

        public void Dispose()
        {
        }

        public Task<IList<SquidexUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<IList<SquidexUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<SquidexUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return apiClient.GetAsync(userId);
        }

        public async Task<SquidexUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var result = await apiClient.GetAsync(filter: $"data/normalizedEmail/iv eq '{normalizedEmail}'");

            return result.Items.SingleOrDefault();
        }

        public async Task<SquidexUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = await apiClient.GetAsync(filter: $"data/normalizedUserName/iv eq '{normalizedUserName}'");

            return result.Items.SingleOrDefault();
        }

        public async Task<SquidexUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var result = await apiClient.GetAsync(filter: $"data/loginKeys/iv eq '{SquidexUserData.LoginKey(loginProvider, providerKey)}'");

            return result.Items.SingleOrDefault();
        }

        public async Task<IdentityResult> CreateAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            var result = await apiClient.CreateAsync(user.Data);

            user.Id = result.Id;

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            await apiClient.UpdateAsync(user);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            await apiClient.DeleteAsync(user);

            return IdentityResult.Success;
        }

        public Task<string> GetUserIdAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.UserName);
        }

        public Task<string> GetNormalizedUserNameAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.PasswordHash);
        }

        public Task<IList<string>> GetRolesAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.GetRoles());
        }

        public Task<bool> IsInRoleAsync(SquidexUser user, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.Roles.Contains(roleName));
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.GetLogins());
        }

        public Task<string> GetSecurityStampAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.SecurityStamp);
        }

        public Task<string> GetEmailAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.NormalizedEmail);
        }

        public Task<IList<Claim>> GetClaimsAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.GetClaims());
        }

        public Task<string> GetPhoneNumberAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.PhoneNumberConfirmed);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.LockoutEndDateUtc);
        }

        public Task<int> GetAccessFailedCountAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.LockoutEnabled);
        }

        public Task<string> GetTokenAsync(SquidexUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Data.GetToken(loginProvider, name));
        }

        public Task<bool> HasPasswordAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.Data.PasswordHash));
        }

        public Task SetUserNameAsync(SquidexUser user, string userName, CancellationToken cancellationToken)
        {
            user.Data.UserName = userName;

            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(SquidexUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Data.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(SquidexUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.Data.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task AddToRoleAsync(SquidexUser user, string roleName, CancellationToken cancellationToken)
        {
            user.Data.AddRole(roleName);

            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(SquidexUser user, string roleName, CancellationToken cancellationToken)
        {
            user.Data.RemoveRole(roleName);

            return Task.CompletedTask;
        }

        public Task AddLoginAsync(SquidexUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            user.Data.AddLogin(login);

            return Task.CompletedTask;
        }

        public Task RemoveLoginAsync(SquidexUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            user.Data.RemoveLogin(loginProvider, providerKey);

            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(SquidexUser user, string stamp, CancellationToken cancellationToken)
        {
            user.Data.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        public Task SetEmailAsync(SquidexUser user, string email, CancellationToken cancellationToken)
        {
            user.Data.Email = email;

            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(SquidexUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.Data.EmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(SquidexUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.Data.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }

        public Task AddClaimsAsync(SquidexUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            user.Data.AddClaims(claims);

            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(SquidexUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            user.Data.ReplaceClaim(claim, newClaim);

            return Task.CompletedTask;
        }

        public Task RemoveClaimsAsync(SquidexUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            user.Data.RemoveClaims(claims);

            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(SquidexUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.Data.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(SquidexUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.Data.PhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(SquidexUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.Data.LockoutEndDateUtc = lockoutEnd?.UtcDateTime;

            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            user.Data.AccessFailedCount++;

            return Task.FromResult(user.Data.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(SquidexUser user, CancellationToken cancellationToken)
        {
            user.Data.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(SquidexUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.Data.LockoutEnabled = enabled;

            return Task.CompletedTask;
        }

        public Task SetTokenAsync(SquidexUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            user.Data.SetToken(loginProvider, name, value);

            return Task.CompletedTask;
        }

        public Task RemoveTokenAsync(SquidexUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            user.Data.RemoveToken(loginProvider, name);

            return Task.CompletedTask;
        }
    }
}
