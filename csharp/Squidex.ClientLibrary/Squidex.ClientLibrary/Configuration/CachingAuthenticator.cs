// ==========================================================================
//  InvariantConverter.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.Configuration
{
    public class CachingAuthenticator : IAuthenticator
    {
        private readonly IMemoryCache cache;
        private readonly IAuthenticator authenticator;
        private readonly string cacheKey;

        public CachingAuthenticator(string cacheKey, IMemoryCache cache, IAuthenticator authenticator)
        {
            Guard.NotNull(cacheKey, nameof(cacheKey));
            Guard.NotNull(cache, nameof(cache));
            Guard.NotNull(authenticator, nameof(authenticator));

            this.cacheKey = cacheKey;
            this.cache = cache;
            this.authenticator = authenticator;
        }

        public async Task<string> GetBearerTokenAsync()
        {
            var result = GetFromCache();

            if (result == null)
            {
                result = await authenticator.GetBearerTokenAsync();

                SetToCache(result, DateTimeOffset.UtcNow.AddDays(50));
            }

            return result;
        }

        public Task RemoveTokenAsync(string token)
        {
            RemoveFromCache();

            return authenticator.RemoveTokenAsync(token);
        }

        protected string GetFromCache()
        {
            return cache.Get<string>(cacheKey);
        }

        protected void RemoveFromCache()
        {
            cache.Remove(cacheKey);
        }

        protected void SetToCache(string result, DateTimeOffset expires)
        {
            cache.Set(cacheKey, result, expires);
        }
    }
}
