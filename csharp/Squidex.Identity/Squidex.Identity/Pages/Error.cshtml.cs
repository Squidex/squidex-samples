// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Squidex.Identity.Pages
{
    public sealed class ErrorModel : PageModel
    {
        private readonly IIdentityServerInteractionService interaction;

        public ErrorModel(IIdentityServerInteractionService interaction)
        {
            this.interaction = interaction;
        }

        public string ErrorMessage { get; set; }

        public string ErrorCode { get; set; } = "400";

        public async Task OnGet(string errorId = null)
        {
            if (!string.IsNullOrWhiteSpace(errorId))
            {
                var context = await interaction.GetErrorContextAsync(errorId);

                ErrorMessage = context?.ErrorDescription;
                ErrorCode = context?.Error ?? "400";
            }

            if (string.IsNullOrWhiteSpace(ErrorMessage))
            {
                var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

                ErrorMessage = exception?.Message;
            }
        }
    }
}
