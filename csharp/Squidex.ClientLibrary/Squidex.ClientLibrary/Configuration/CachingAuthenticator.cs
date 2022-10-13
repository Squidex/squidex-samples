﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.Configuration
{
    /// <summary>
    /// An authenticator that stores the JWT token in the memory cache.
    /// </summary>
    /// <seealso cref="IAuthenticator" />
    public class CachingAuthenticator : IAuthenticator
    {
        private readonly IAuthenticator authenticator;
        private DateTimeOffset cacheExpires;
        private string? cacheEntry;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingAuthenticator"/> class with the cache key,
        /// the memory cache and inner authenticator that does the actual work.
        /// </summary>
        /// <param name="authenticator">The inner authenticator that does the actual work.  Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="authenticator"/> is null.</exception>
        public CachingAuthenticator(IAuthenticator authenticator)
        {
            Guard.NotNull(authenticator, nameof(authenticator));

            this.authenticator = authenticator;
        }

        /// <inheritdoc/>
        public async Task<string> GetBearerTokenAsync(
            CancellationToken ct)
        {
            var result = GetFromCache();

            if (result == null)
            {
                result = await authenticator.GetBearerTokenAsync(ct);

                SetToCache(result, DateTimeOffset.UtcNow.AddDays(50));
            }

            return result;
        }

        /// <inheritdoc/>
        public Task RemoveTokenAsync(string token,
            CancellationToken ct)
        {
            RemoveFromCache();

            return authenticator.RemoveTokenAsync(token, ct);
        }

        /// <inheritdoc/>
        public bool ShouldIntercept(HttpRequestMessage request)
        {
            var shouldIntercept = authenticator.ShouldIntercept(request);

            return shouldIntercept;
        }

        /// <summary>
        /// Gets the current JWT bearer token from the cache.
        /// </summary>
        /// <returns>
        /// The JWT bearer token or null if not found in the cache.
        /// </returns>
        protected string? GetFromCache()
        {
            if (cacheExpires < DateTimeOffset.UtcNow)
            {
                RemoveFromCache();
            }

            return cacheEntry;
        }

        /// <summary>
        /// Removes from current JWT bearer token from the cache.
        /// </summary>
        protected void RemoveFromCache()
        {
            cacheExpires = default;
            cacheEntry = default;
        }

        /// <summary>
        /// Sets to the current JWT bearer token.
        /// </summary>
        /// <param name="token">The JWT bearer token.</param>
        /// <param name="expires">The date and time when the token will expire.</param>
        protected void SetToCache(string token, DateTimeOffset expires)
        {
            cacheExpires = expires;
            cacheEntry = token;
        }
    }
}
