// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squidex.Identity.Extensions;

namespace Squidex.Identity.Pages.Manage
{
    public sealed class ChangePasswordModel : ManagePageModelBase<ChangePasswordModel>
    {
        [BindProperty]
        public ChangePasswordInputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await UserManager.HasPasswordAsync(UserInfo))
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.ChangePasswordAsync(UserInfo, Input.OldPassword, Input.NewPassword);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(UserInfo, false);

                    StatusMessage = T["PasswordChanged"];

                    return RedirectToPage();
                }

                ModelState.AddModelErrors(result);
            }

            return Page();
        }
    }

    public sealed class ChangePasswordInputModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "PasswordsNotSame")]
        public string ConfirmPassword { get; set; }
    }
}
