// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Squidex.Identity.Model.Authentication
{
    public static class Extensions
    {
        public static void AddAuthenticationConfigurator<TOptions, THandler>(this IServiceCollection services, AuthenticationSchemeType type, Func<AuthenticationSchemeData, TOptions> factory)
            where TOptions : AuthenticationSchemeOptions, new()
            where THandler : AuthenticationHandler<TOptions>
        {
            services.AddSingleton(c => new AuthenticationSchemeConfigurator<TOptions, THandler>(type,
                c.GetRequiredService<IAuthenticationSchemeStore>(),
                c.GetRequiredService<IEnumerable<IPostConfigureOptions<TOptions>>>(),
                factory));

            services.AddSingleton<IAuthenticationSchemeConfigurator>(c =>
                c.GetRequiredService<AuthenticationSchemeConfigurator<TOptions, THandler>>());

            services.AddSingleton<IOptionsMonitor<TOptions>>(c =>
                c.GetRequiredService<AuthenticationSchemeConfigurator<TOptions, THandler>>());
        }
    }
}
