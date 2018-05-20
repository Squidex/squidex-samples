// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Squidex.Identity.Model;

namespace Squidex.Identity.Extensions
{
    public abstract class PageModelBase<TDerived> : PageModel
    {
        private readonly Lazy<SignInManager<UserEntity>> signInManager;
        private readonly Lazy<UserManager<UserEntity>> userManager;
        private readonly Lazy<ILogger<TDerived>> logger;
        private readonly Lazy<ISettingsProvider> settings;
        private readonly Lazy<IStringLocalizer<AppResources>> localizer;

        public SignInManager<UserEntity> SignInManager
        {
            get { return signInManager.Value; }
        }

        public UserManager<UserEntity> UserManager
        {
            get { return userManager.Value; }
        }

        public ILogger<TDerived> Logger
        {
            get { return logger.Value; }
        }

        public ISettingsProvider Settings
        {
            get { return settings.Value; }
        }

        public IStringLocalizer<AppResources> T
        {
            get { return localizer.Value; }
        }

        [TempData]
        public string StatusMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        protected PageModelBase()
        {
            SetupService(ref localizer);
            SetupService(ref logger);
            SetupService(ref settings);
            SetupService(ref signInManager);
            SetupService(ref userManager);
        }

        private void SetupService<TService>(ref Lazy<TService> value)
        {
#pragma warning disable RECS0002 // Convert anonymous method to method group
            value = new Lazy<TService>(() => HttpContext.RequestServices.GetRequiredService<TService>());
#pragma warning restore RECS0002 // Convert anonymous method to method group
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);
        }

        protected IActionResult RedirectTo(string returnUrl)
        {
            if (Uri.IsWellFormedUriString(returnUrl, UriKind.RelativeOrAbsolute))
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return Redirect("~/");
            }
        }

        protected async Task<UserEntity> GetUserAsync()
        {
            var user = await UserManager.GetUserAsync(User);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
            }

            return user;
        }
    }
}
