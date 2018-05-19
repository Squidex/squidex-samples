// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Squidex.Identity.Extensions;
using Squidex.Identity.Model;

namespace Squidex.Identity.Pages.Manage
{
    public sealed class SetPasswordModel : PageModelBase<SetPasswordModel>
    {
        [BindProperty]
        public SetPasswordInputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public UserEntity UserInfo { get; set; }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            UserInfo = await GetUserAsync();

            await next();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (await UserManager.HasPasswordAsync(UserInfo))
            {
                return RedirectToPage("./ChangePassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(UserInfo, Input.NewPassword);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(UserInfo, false);

                    StatusMessage = T["PasswordSet"];

                    return RedirectToPage();
                }

                ModelState.AddModelErrors(result);
            }

            return Page();
        }
    }

    public sealed class SetPasswordInputModel
    {
        [Required, StringLength(100,  MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "PasswordsNotSame")]
        public string ConfirmPassword { get; set; }
    }
}
