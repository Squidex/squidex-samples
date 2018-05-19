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

namespace Squidex.Identity.Pages.Manage
{
    public sealed class IndexModel : PageModelBase<IndexModel>
    {
        private readonly IEmailSender emailSender;

        public IndexModel(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        [BindProperty]
        public ProfileInputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public UserEntity UserInfo { get; set; }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            UserInfo = await GetUserAsync();

            await next();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Input = new ProfileInputModel { Email = UserInfo.Data.Email };

            IsEmailConfirmed = await UserManager.IsEmailConfirmedAsync(UserInfo);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (!string.Equals(Input.Email, UserInfo.Data.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var result = await UserManager.SetEmailAsync(UserInfo, Input.Email);

                    if (!result.Succeeded)
                    {
                        throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{UserInfo.Id}'.");
                    }

                    StatusMessage = T["ProfileUpdated"];
                }

                return RedirectToPage();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (ModelState.IsValid)
            {
                var callbackCode = await UserManager.GenerateEmailConfirmationTokenAsync(UserInfo);
                var callbackUrl = Url.EmailConfirmationLink(UserInfo.Id, callbackCode, Request.Scheme);

                StatusMessage = T["VerificationEmailSent"];

                return RedirectToPage();
            }

            return Page();
        }
    }

    public sealed class ProfileInputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
