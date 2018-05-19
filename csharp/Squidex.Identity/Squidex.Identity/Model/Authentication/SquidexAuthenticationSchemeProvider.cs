// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.Extensions.Options;

namespace Squidex.Identity.Model.Authentication
{
    public sealed class SquidexAuthenticationSchemeProvider : IAuthenticationSchemeProvider
    {
        private readonly List<AuthenticationScheme> defaultSchemes = new List<AuthenticationScheme>();
        private readonly IOptions<AuthenticationOptions> options;
        private readonly IAuthenticationSchemeStore store;

        public SquidexAuthenticationSchemeProvider(
            IAuthenticationSchemeStore store,
            IEnumerable<IAuthenticationSchemeConfigurator> configurators,
            IOptions<AuthenticationOptions> options)
        {
            this.options = options;

            foreach (var builder in options.Value.Schemes)
            {
                var scheme = builder.Build();

                if (!configurators.Any(x => x.HandlerType == scheme.HandlerType))
                {
                    defaultSchemes.Add(scheme);
                }
            }

            this.store = store;
        }

        public void AddScheme(AuthenticationScheme scheme)
        {
            throw new NotSupportedException();
        }

        public void RemoveScheme(string name)
        {
            throw new NotSupportedException();
        }

        private Task<AuthenticationScheme> GetDefaultSchemeAsync()
        {
            return options.Value.DefaultScheme != null ? GetSchemeAsync(options.Value.DefaultScheme) : Task.FromResult<AuthenticationScheme>(null);
        }

        private Task<AuthenticationScheme> GetSchemaOrDefaultAsync(string name)
        {
            return name != null ? GetSchemeAsync(name) : GetDefaultSchemeAsync();
        }

        public Task<AuthenticationScheme> GetDefaultAuthenticateSchemeAsync()
        {
            return GetSchemaOrDefaultAsync(options.Value.DefaultAuthenticateScheme);
        }

        public Task<AuthenticationScheme> GetDefaultChallengeSchemeAsync()
        {
            return GetSchemaOrDefaultAsync(options.Value.DefaultChallengeScheme);
        }

        public Task<AuthenticationScheme> GetDefaultForbidSchemeAsync()
        {
            return GetSchemaOrDefaultAsync(options.Value.DefaultForbidScheme);
        }

        public Task<AuthenticationScheme> GetDefaultSignInSchemeAsync()
        {
            return GetSchemaOrDefaultAsync(options.Value.DefaultSignInScheme);
        }

        public Task<AuthenticationScheme> GetDefaultSignOutSchemeAsync()
        {
            return GetSchemaOrDefaultAsync(options.Value.DefaultSignOutScheme);
        }

        public async Task<IEnumerable<AuthenticationScheme>> GetRequestHandlerSchemesAsync()
        {
            var schemes = await GetAllSchemesAsync();

            return schemes.Where(x => typeof(IAuthenticationRequestHandler).IsAssignableFrom(x.HandlerType));
        }

        public async Task<AuthenticationScheme> GetSchemeAsync(string name)
        {
            var schemes = await GetAllSchemesAsync();

            return schemes.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync()
        {
            var schemes = await store.GetSchemesAsync();

            var result = new List<AuthenticationScheme>(defaultSchemes);

            foreach (var scheme in schemes)
            {
                switch (scheme.Provider)
                {
                    case AuthenticationSchemeType.Facebook:
                        result.Add(new AuthenticationScheme(FacebookDefaults.AuthenticationScheme, "Facebook", typeof(FacebookHandler)));
                        break;
                    case AuthenticationSchemeType.Google:
                        result.Add(new AuthenticationScheme(GoogleDefaults.AuthenticationScheme, "Google", typeof(GoogleHandler)));
                        break;
                    case AuthenticationSchemeType.Microsoft:
                        result.Add(new AuthenticationScheme(MicrosoftAccountDefaults.AuthenticationScheme, "Microsoft", typeof(MicrosoftAccountHandler)));
                        break;
                    case AuthenticationSchemeType.Twitter:
                        result.Add(new AuthenticationScheme(TwitterDefaults.AuthenticationScheme, "Twitter", typeof(TwitterHandler)));
                        break;
                }
            }

            return result;
        }
    }
}
