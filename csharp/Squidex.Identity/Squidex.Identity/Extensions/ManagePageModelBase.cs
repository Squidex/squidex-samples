// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Squidex.Identity.Model;

namespace Squidex.Identity.Extensions
{
    public abstract class ManagePageModelBase<TDerived> : PageModelBase<TDerived>
    {
        public UserEntity UserInfo { get; set; }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            UserInfo = await GetUserAsync();

            await base.OnPageHandlerExecutionAsync(context, next);
        }
    }
}
