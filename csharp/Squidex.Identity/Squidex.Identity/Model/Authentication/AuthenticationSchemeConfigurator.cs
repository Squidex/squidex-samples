// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Squidex.Identity.Model.Authentication
{
    public sealed class AuthenticationSchemeConfigurator<TOptions, THandler> : IOptionsMonitor<TOptions>, IAuthenticationSchemeConfigurator
        where TOptions : AuthenticationSchemeOptions, new()
        where THandler : AuthenticationHandler<TOptions>
    {
        private readonly AuthenticationSchemeType authenticationSchemeType;
        private readonly IAuthenticationSchemeStore authenticationSchemeStore;
        private readonly IEnumerable<IPostConfigureOptions<TOptions>> postConfigurations;
        private readonly Func<AuthenticationSchemeData, TOptions> factory;

        public Type HandlerType
        {
            get { return typeof(THandler); }
        }

        public AuthenticationSchemeConfigurator(
            AuthenticationSchemeType authenticationSchemeType,
            IAuthenticationSchemeStore authenticationSchemeStore,
            IEnumerable<IPostConfigureOptions<TOptions>> postConfigurations,
            Func<AuthenticationSchemeData, TOptions> factory)
        {
            this.authenticationSchemeType = authenticationSchemeType;
            this.authenticationSchemeStore = authenticationSchemeStore;

            this.postConfigurations = postConfigurations;

            this.factory = factory;
        }

        public TOptions CurrentValue
        {
            get { return Get(Options.DefaultName); }
        }

        public TOptions Get(string name)
        {
            var scheme = authenticationSchemeStore.GetSchemesAsync().Result.FirstOrDefault(x => x.Provider == authenticationSchemeType);

            if (scheme != null)
            {
                var result = factory(scheme);

                foreach (var configurator in postConfigurations)
                {
                    configurator.PostConfigure(name, result);
                }

                return result;
            }

            return null;
        }

        public IDisposable OnChange(Action<TOptions, string> listener)
        {
            return null;
        }
    }
}
