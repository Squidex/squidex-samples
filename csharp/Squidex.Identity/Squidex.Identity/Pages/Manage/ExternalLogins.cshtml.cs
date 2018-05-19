// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squidex.Identity.Extensions;

namespace Squidex.Identity.Pages.Manage
{
    public sealed class ExternalLoginsModel : ManagePageModelBase<ExternalLoginsModel>
    {
        public bool ShowRemoveButton { get; set; }

        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentLogins = await UserManager.GetLoginsAsync(UserInfo);

            OtherLogins =
                (await SignInManager.GetExternalAuthenticationSchemesAsync())
                    .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider)).ToList();

            ShowRemoveButton = UserInfo.Data.PasswordHash != null || CurrentLogins.Count > 1;

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var result = await UserManager.RemoveLoginAsync(UserInfo, loginProvider, providerKey);

            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{UserInfo.Id}'.");
            }

            await SignInManager.SignInAsync(UserInfo, false);

            StatusMessage = T["ExternalLoginRemoved"];

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var authenticationRedirectUrl = Url.Page("./ExternalLogins", "LinkLoginCallback");
            var authenticationProperties = SignInManager.ConfigureExternalAuthenticationProperties(provider, authenticationRedirectUrl, UserInfo.Id);

            return new ChallengeResult(provider, authenticationProperties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var loginInfo = await SignInManager.GetExternalLoginInfoAsync(UserInfo.Id);

            if (loginInfo == null)
            {
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{UserInfo.Id}'.");
            }

            var result = await UserManager.AddLoginAsync(UserInfo, loginInfo);

            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{UserInfo.Id}'.");
            }

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = T["ExternalLoginAdded"];

            return RedirectToPage();
        }
    }
}
