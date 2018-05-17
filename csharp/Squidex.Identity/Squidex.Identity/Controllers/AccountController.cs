// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squidex.Identity.Model;

namespace Squidex.Identity.Controllers
{
    public sealed class AccountController : Controller
    {
        private readonly SignInManager<UserEntity> signInManager;

        public AccountController(SignInManager<UserEntity> signInManager)
        {
            this.signInManager = signInManager;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return RedirectToPage("/login");
        }

        [HttpPost]
        [Route("/logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToPage("/login");
        }
    }
}
