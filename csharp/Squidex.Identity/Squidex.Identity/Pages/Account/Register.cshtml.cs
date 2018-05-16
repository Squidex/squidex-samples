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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Squidex.Identity.Model;
using Squidex.Identity.Services;

namespace Squidex.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<SquidexUser> signInManager;
        private readonly UserManager<SquidexUser> userManager;
        private readonly ILogger<LoginModel> logger;
        private readonly IEmailSender emailSender;

        public RegisterModel(
            UserManager<SquidexUser> userManager,
            SignInManager<SquidexUser> signInManager,
            ILogger<LoginModel> logger,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            public string Password { get; set; }

            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new SquidexUser();
                user.Data.Email = Input.Email;
                user.Data.UserName = Input.Email;

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
}
