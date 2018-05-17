// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Squidex.Identity.Model;
using Squidex.Identity.Services;

namespace Squidex.Identity.Pages
{
    public sealed class RegisterModel : PageModel
    {
        private readonly SignInManager<UserEntity> signInManager;
        private readonly ISettingsProvider settingsProvider;
        private readonly UserManager<UserEntity> userManager;
        private readonly ILogger<LoginModel> logger;
        private readonly IStringLocalizer<AppResources> localizer;
        private readonly IEmailSender emailSender;

        public RegisterModel(
            IEmailSender emailSender,
            ILogger<LoginModel> logger,
            ISettingsProvider settingsProvider,
            IStringLocalizer<AppResources> localizer,
            SignInManager<UserEntity> signInManager,
            UserManager<UserEntity> userManager)
        {
            this.emailSender = emailSender;
            this.localizer = localizer;
            this.logger = logger;
            this.settingsProvider = settingsProvider;
            this.signInManager = signInManager;
            this.userManager = userManager;
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
            var settings = await settingsProvider.GetSettingsAsync();

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

                ModelState.AddModelError($"{nameof(Input)}.{field}", localizer[$"{field}Error"]);
            }

            if (MustAcceptsTermsOfService && !Input.AcceptTermsOfService)
            {
                var field = nameof(Input.AcceptTermsOfService);

                ModelState.AddModelError($"{nameof(Input)}.{field}", localizer[$"{field}Error"]);
            }

            if (ModelState.IsValid)
            {
                var user = UserEntity.Create(Input.Email);

                var result = await userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var callbackCode = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, callbackCode, Request.Scheme);

                    await emailSender.SendEmailConfirmationAsync(Input.Email, callbackUrl);

                    await signInManager.SignInAsync(user, false);

                    return LocalRedirect(ReturnUrl);
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
