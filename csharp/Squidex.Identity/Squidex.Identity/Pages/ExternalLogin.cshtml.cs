// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Squidex.Identity.Extensions;
using Squidex.Identity.Model;

namespace Squidex.Identity.Pages
{
    public sealed class ExternalLoginModel : PageModelBase<ExternalLoginModel>
    {
        [BindProperty]
        public ExternalLoginInputModel Input { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        public string LoginProvider { get; set; }

        public string TermsOfServiceUrl { get; set; }

        public string PrivacyPolicyUrl { get; set; }

        public bool MustAcceptsTermsOfService { get; set; }

        public bool MustAcceptsPrivacyPolicy { get; set; }

        public async override Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var settings = await Settings.GetSettingsAsync();

            if (!string.IsNullOrWhiteSpace(settings.PrivacyPolicyUrl))
            {
                PrivacyPolicyUrl = settings.PrivacyPolicyUrl;

                MustAcceptsPrivacyPolicy = true;
            }

            if (!string.IsNullOrWhiteSpace(settings.TermsOfServiceUrl))
            {
                TermsOfServiceUrl = settings.TermsOfServiceUrl;

                MustAcceptsTermsOfService = true;
            }

            await next();
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider)
        {
            var authenticationRedirectUrl = Url.Page("./ExternalLogin", "Callback", new { returnUrl = ReturnUrl });
            var authenticationProperties = SignInManager.ConfigureExternalAuthenticationProperties(provider, authenticationRedirectUrl);

            return new ChallengeResult(provider, authenticationProperties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = T["ExternalLoginError", remoteError];

                return RedirectToPage("./Login");
            }

            var loginInfo = await SignInManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                return RedirectToPage("./Login");
            }

            var result = await SignInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, false);

            if (result.Succeeded)
            {
                return RedirectTo(ReturnUrl);
            }
            else if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            LoginProvider = loginInfo.LoginProvider;

            if (loginInfo.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                Input = new ExternalLoginInputModel
                {
                    Email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email)
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmationAsync()
        {
            if (MustAcceptsPrivacyPolicy && !Input.AcceptPrivacyPolicy)
            {
                var field = nameof(Input.AcceptPrivacyPolicy);

                ModelState.AddModelError($"{nameof(Input)}.{field}", T[$"{field}Error"]);
            }

            if (MustAcceptsTermsOfService && !Input.AcceptTermsOfService)
            {
                var field = nameof(Input.AcceptTermsOfService);

                ModelState.AddModelError($"{nameof(Input)}.{field}", T[$"{field}Error"]);
            }

            if (ModelState.IsValid)
            {
                var loginInfo = await SignInManager.GetExternalLoginInfoAsync();

                if (loginInfo == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }

                var user = UserEntity.Create(Input.Email);

                var result = await UserManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user, loginInfo);

                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, false);

                        return RedirectTo(ReturnUrl);
                    }
                }

                ModelState.AddModelErrors(result);
            }

            return Page();
        }
    }

    public sealed class ExternalLoginInputModel
    {
        [Required, EmailAddress]
        [Display(Name = nameof(Email))]
        public string Email { get; set; }

        [Required]
        [Display(Name = nameof(AcceptPrivacyPolicy))]
        public bool AcceptPrivacyPolicy { get; set; }

        [Required]
        [Display(Name = nameof(AcceptTermsOfService))]
        public bool AcceptTermsOfService { get; set; }
    }
}
