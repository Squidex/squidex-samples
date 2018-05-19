// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Squidex.Identity.Extensions;

namespace Squidex.Identity.Pages
{
    public sealed class ConfirmEmailModel : PageModelBase<ConfirmEmailModel>
    {
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return RedirectToPage("/Index");
            }

            var user = await UserManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await UserManager.ConfirmEmailAsync(user, code);

                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Error confirming email for user with ID '{userId}':");
                }
            }

            return Page();
        }
    }
}
