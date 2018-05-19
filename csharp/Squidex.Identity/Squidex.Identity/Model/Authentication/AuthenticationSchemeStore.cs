// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Squidex.ClientLibrary;

namespace Squidex.Identity.Model.Authentication
{
    public sealed class AuthenticationSchemeStore : CachingProvider, IAuthenticationSchemeStore
    {
        private readonly SquidexClient<AuthenticationSchemeEntity, AuthenticationSchemeData> apiClient;

        public AuthenticationSchemeStore(SquidexClientManager clientManager, IMemoryCache cache)
            : base(cache)
        {
            apiClient = clientManager.GetClient<AuthenticationSchemeEntity, AuthenticationSchemeData>("authentication-schemes");
        }

        public Task<List<AuthenticationSchemeData>> GetSchemesAsync()
        {
            return GetOrAddAsync(nameof(AuthenticationSchemeType), async () =>
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
