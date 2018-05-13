// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Squidex.Identity.Model;

namespace Squidex.Identity.Pages.Account.Manage
{
    public class TwoFactorAuthenticationModel : PageModel
    {
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}";

        private readonly UserManager<SquidexUser> userManager;
        private readonly SignInManager<SquidexUser> signInManager;
        private readonly ILogger<TwoFactorAuthenticationModel> logger;

        public TwoFactorAuthenticationModel(
            UserManager<SquidexUser> userManager,
            SignInManager<SquidexUser> signInManager,
            ILogger<TwoFactorAuthenticationModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        [BindProperty]
        public bool Is2faEnabled { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) != null;
            Is2faEnabled = await userManager.GetTwoFactorEnabledAsync(user);
            RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user);

            return Page();
        }
    }
}