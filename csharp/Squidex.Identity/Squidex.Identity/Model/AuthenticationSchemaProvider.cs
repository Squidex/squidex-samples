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
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Squidex.ClientLibrary;

namespace Squidex.Identity.Model
{
    public sealed class AuthenticationSchemaProvider : CachingProvider,
        IAuthenticationSchemeProvider,
        IOptionsMonitor<GoogleOptions>,
        IOptionsMonitor<MicrosoftAccountOptions>
    {
        private static readonly Type GoogleHandlerType =
            typeof(GoogleOptions).Assembly.GetType(" Microsoft.AspNetCore.Authentication.Google.GoogleHandler", true, true);
        private static readonly Type MicrosoftAccountHandlerType =
            typeof(MicrosoftAccountOptions).Assembly.GetType("Microsoft.AspNetCore.Authentication.MicrosoftAccount.MicrosoftAccountHandler", true, true);

        private readonly List<AuthenticationScheme> defaultSchemes = new List<AuthenticationScheme>();
        private readonly SquidexClient<AuthenticationSchemeEntity, AuthenticationSchemeData> apiClient;
        private readonly IOptions<AuthenticationOptions> options;

        public AuthenticationSchemaProvider(SquidexClientManager clientManager, IMemoryCache cache, IOptions<AuthenticationOptions> options)
            : base(cache)
        {
            apiClient = clientManager.GetClient<AuthenticationSchemeEntity, AuthenticationSchemeData>("authentication-schemes");

            foreach (var builder in options.Value.Schemes)
            {
                var scheme = builder.Build();

                if (scheme.HandlerType != GoogleHandlerType &&
                    scheme.HandlerType != MicrosoftAccountHandlerType)
                {
                    defaultSchemes.Add(scheme);
                }
            }

            this.options = options;
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

        MicrosoftAccountOptions IOptionsMonitor<MicrosoftAccountOptions>.CurrentValue
        {
            get { return ((IOptionsMonitor<MicrosoftAccountOptions>)this).Get(Options.DefaultName); }
        }

        GoogleOptions IOptionsMonitor<GoogleOptions>.CurrentValue
        {
            get { return ((IOptionsMonitor<GoogleOptions>)this).Get(Options.DefaultName); }
        }

        IDisposable IOptionsMonitor<GoogleOptions>.OnChange(Action<GoogleOptions, string> listener)
        {
            return null;
        }

        IDisposable IOptionsMonitor<MicrosoftAccountOptions>.OnChange(Action<MicrosoftAccountOptions, string> listener)
        {
            return null;
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
            var schemes = await GetSchemesAsync();

            var result = new List<AuthenticationScheme>(defaultSchemes);

            foreach (var scheme in schemes)
            {
                if (scheme.Provider == AuthenticationSchemeProvider.Google)
                {
                    result.Add(new AuthenticationScheme(GoogleDefaults.AuthenticationScheme, "Google", GoogleHandlerType));
                }
                else if (scheme.Provider == AuthenticationSchemeProvider.MicrosoftAccount)
                {
                    result.Add(new AuthenticationScheme(MicrosoftAccountDefaults.AuthenticationScheme, "Microsoft", MicrosoftAccountHandlerType));
                }
            }

            return result;
        }

        GoogleOptions IOptionsMonitor<GoogleOptions>.Get(string name)
        {
            var scheme = GetSchemesAsync().Result.FirstOrDefault(x => x.Provider == AuthenticationSchemeProvider.Google);

            if (scheme != null)
            {
                return new GoogleOptions
                {
                    ClientId = scheme.ClientId,
                    ClientSecret = scheme.ClientSecret
                };
            }

            return null;
        }

        MicrosoftAccountOptions IOptionsMonitor<MicrosoftAccountOptions>.Get(string name)
        {
            var scheme = GetSchemesAsync().Result.FirstOrDefault(x => x.Provider == AuthenticationSchemeProvider.MicrosoftAccount);

            if (scheme != null)
            {
                return new MicrosoftAccountOptions
                {
                    ClientId = scheme.ClientId,
                    ClientSecret = scheme.ClientSecret
                };
            }

            return null;
        }

        private Task<List<AuthenticationSchemeData>> GetSchemesAsync()
        {
            return GetOrAddAsync(nameof(AuthenticationSchemeProvider), async () =>
            {
                var schemes = await apiClient.GetAsync();

                return schemes.Items
                    .Select(x => x.Data).GroupBy(x => x.Provider)
                    .Select(x => x.First())
                    .ToList();
            });
        }
    }
}
