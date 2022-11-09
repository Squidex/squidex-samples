// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.Configuration;

/// <summary>
/// An authenticator that stores the JWT token in the memory cache.
/// </summary>
/// <seealso cref="IAuthenticator" />
public class CachingAuthenticator : IAuthenticator
{
    private readonly IAuthenticator authenticator;
    private readonly Cache<string, string?> cache = new Cache<string, string?>();

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

            cache.Set(appName, result, TimeSpan.FromDays(50));
        }

        return result;
    }

    /// <inheritdoc/>
    public Task RemoveTokenAsync(string appName, string token,
        CancellationToken ct)
    {
        cache.Remove(appName);

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
        cache.TryGet(appName, out var token);

        return token;
    }

    /// <summary>
    /// Removes from current JWT bearer token from the cache.
    /// </summary>
    /// <param name="appName">The name of the app.</param>
    public void RemoveFromCache(string appName)
    {
        cache.Remove(appName);
    }

    /// <summary>
    /// Sets to the current JWT bearer token.
    /// </summary>
    /// <param name="appName">The name of the app.</param>
    /// <param name="token">The JWT bearer token.</param>
    /// <param name="expires">The date and time when the token will expire.</param>
    public void SetToCache(string appName, string token, DateTimeOffset expires)
    {
        cache.Set(appName, token, expires);
    }
}
