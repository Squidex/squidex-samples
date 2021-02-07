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
    /// <summary>
    /// An authenticator that stores the JWT token in the memory cache.
    /// </summary>
    /// <seealso cref="IAuthenticator" />
    public class CachingAuthenticator : IAuthenticator
    {
        private readonly IMemoryCache cache;
        private readonly IAuthenticator authenticator;
        private readonly string cacheKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingAuthenticator"/> class with the cache key,
        /// the memory cache and inner authenticator that does the actual work.
        /// </summary>
        /// <param name="cacheKey">The cache key. Cannot be null or empty.</param>
        /// <param name="cache">The memory cache. Cannot be null.</param>
        /// <param name="authenticator">The inner authenticator that does the actual work.  Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cacheKey"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="cache"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="authenticator"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="cacheKey"/> is empty.</exception>
        public CachingAuthenticator(string cacheKey, IMemoryCache cache, IAuthenticator authenticator)
        {
            Guard.NotNull(cacheKey, nameof(cacheKey));
            Guard.NotNull(cache, nameof(cache));
            Guard.NotNull(authenticator, nameof(authenticator));

            this.cacheKey = cacheKey;
            this.cache = cache;
            this.authenticator = authenticator;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public Task RemoveTokenAsync(string token)
        {
            RemoveFromCache();

            return authenticator.RemoveTokenAsync(token);
        }

        /// <summary>
        /// Gets the current JWT bearer token from the cache.
        /// </summary>
        /// <returns>
        /// The JWT bearer token or null if not found in the cache.
        /// </returns>
        protected string GetFromCache()
        {
            return cache.Get<string>(cacheKey);
        }

        /// <summary>
        /// Removes from current JWT bearer token from the cache.
        /// </summary>
        protected void RemoveFromCache()
        {
            cache.Remove(cacheKey);
        }

        /// <summary>
        /// Sets to the current JWT bearer token.
        /// </summary>
        /// <param name="token">The JWT bearer token.</param>
        /// <param name="expires">The date and time when the token will expire..</param>
        protected void SetToCache(string token, DateTimeOffset expires)
        {
            cache.Set(cacheKey, token, expires);
        }
    }
}
