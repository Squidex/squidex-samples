// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Squidex.Identity.Extensions;
using Squidex.Identity.Model;
using Squidex.Identity.Services;

namespace Squidex.Identity.Pages
{
    public sealed class RegisterModel : PageModelBase<RegisterModel>
    {
        private readonly IEmailSender emailSender;

        public RegisterModel(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        [BindProperty]
        public RequestInputModel Input { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
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
                var user = UserEntity.Create(Input.Email);

                var result = await UserManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var callbackCode = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, callbackCode, Request.Scheme);

                    await emailSender.SendEmailConfirmationAsync(Input.Email, callbackUrl);

                    await SignInManager.SignInAsync(user, false);

                    if (Uri.IsWellFormedUriString(ReturnUrl, UriKind.Absolute))
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToPage("./Index");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }

    public sealed class RequestInputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool AcceptPrivacyPolicy { get; set; }

        [Required]
        public bool AcceptTermsOfService { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "PasswordsNotSame")]
        public string ConfirmPassword { get; set; }
    }
}
