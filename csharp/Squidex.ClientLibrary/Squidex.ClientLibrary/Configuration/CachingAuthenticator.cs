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
        private readonly Dictionary<string, CacheEntry> cache = new Dictionary<string, CacheEntry>(StringComparer.OrdinalIgnoreCase);

        private sealed class CacheEntry
        {
            public string? Token { get; set; }

            public DateTimeOffset Expires { get; set; }
        }

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
        public async Task<string> GetBearerTokenAsync(string appName,
            CancellationToken ct)
        {
            var result = GetFromCache(appName);

            if (result == null)
            {
                result = await authenticator.GetBearerTokenAsync(appName, ct);

                SetToCache(appName, result, DateTimeOffset.UtcNow.AddDays(50));
            }

            return result;
        }

        /// <inheritdoc/>
        public Task RemoveTokenAsync(string appName, string token,
            CancellationToken ct)
        {
            RemoveFromCache(appName);

            return authenticator.RemoveTokenAsync(appName, token, ct);
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
        /// <param name="appName">The name of the app.</param>
        /// <returns>
        /// The JWT bearer token or null if not found in the cache.
        /// </returns>
        protected string? GetFromCache(string appName)
        {
            CacheEntry? entry = null;

            lock (cache)
            {
                cache.TryGetValue(appName, out entry);
            }

            if (entry == null)
            {
                return null;
            }

            if (entry.Expires < DateTimeOffset.UtcNow)
            {
                RemoveFromCache(appName);
            }

            return entry.Token;
        }

        /// <summary>
        /// Removes from current JWT bearer token from the cache.
        /// </summary>
        /// <param name="appName">The name of the app.</param>
        protected void RemoveFromCache(string appName)
        {
            lock (cache)
            {
                cache.Remove(appName);
            }
        }

        /// <summary>
        /// Sets to the current JWT bearer token.
        /// </summary>
        /// <param name="appName">The name of the app.</param>
        /// <param name="token">The JWT bearer token.</param>
        /// <param name="expires">The date and time when the token will expire.</param>
        protected void SetToCache(string appName, string token, DateTimeOffset expires)
        {
            lock (cache)
            {
                cache[appName] = new CacheEntry { Token = token, Expires = expires };
            }
        }
    }
}
