// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Squidex.Identity.Model;

namespace Squidex.Identity.Controllers
{
    public sealed class AccountController : Controller
    {
        private readonly SignInManager<UserEntity> signInManager;
        private readonly IIdentityServerInteractionService interactions;

        public AccountController(SignInManager<UserEntity> signInManager, IIdentityServerInteractionService interactions)
        {
            this.signInManager = signInManager;
            this.interactions = interactions;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("~/manage");
            }
            else
            {
                return Redirect("~/login");
            }
        }

        [Route("logout")]
        public async Task<IActionResult> Logout(string logoutId = null)
        {
            var context = await interactions.GetLogoutContextAsync(logoutId);

            await signInManager.SignOutAsync();

            if (!string.IsNullOrWhiteSpace(context?.PostLogoutRedirectUri))
            {
                return Redirect(context.PostLogoutRedirectUri);
            }
            else
            {
                return Redirect("~/login");
            }
        }
    }
}
