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
using Squidex.Identity.Extensions;

namespace Squidex.Identity.Pages
{
    public sealed class ResetPasswordModel : PageModelBase<ResetPasswordModel>
    {
        [BindProperty]
        public ResetPasswordInputModel Input { get; set; }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }

            Input = new ResetPasswordInputModel { Code = code };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(Input.Email);

                if (user == null)
                {
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                var result = await UserManager.ResetPasswordAsync(user, Input.Code, Input.Password);

                if (result.Succeeded)
                {
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }

    public sealed class ResetPasswordInputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "PasswordsNotSame")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
