// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Security;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.Configuration;

/// <summary>
/// THe default implementation of the <see cref="IAuthenticator"/> interface that makes POST
/// requests to retrieve the JWT bearer token from the connect endpoint.
/// </summary>
/// <seealso cref="IAuthenticator" />
public class Authenticator : IAuthenticator
{
    private const string TokenUrl = "identity-server/connect/token";
    private readonly SquidexOptions options;
    private readonly Cache<(string, string), Exception> invalidAttempts = new Cache<(string, string), Exception>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Authenticator"/> class.
    /// </summary>
    /// <param name="options">The options to configure.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public Authenticator(SquidexOptions options)
    {
        Guard.NotNull(options, nameof(options));

        this.options = options;
    }

    /// <inheritdoc/>
    public bool ShouldIntercept(HttpRequestMessage request)
    {
#if NETSTANDARD2_0
        return !request.RequestUri.PathAndQuery.ToLowerInvariant().Contains(TokenUrl);
#else
        return request.RequestUri?.PathAndQuery.Contains(TokenUrl, StringComparison.OrdinalIgnoreCase) != true;
#endif
    }

    /// <inheritdoc/>
    public Task RemoveTokenAsync(string appName, AuthToken token,
        CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<AuthToken> GetAuthTokenAsync(string appName,
        CancellationToken ct)
    {
        var httpClient = options.ClientProvider.Get();

        var clientId = options.ClientId;
        var clientSecret = options.ClientSecret;

        ThrowFromPreviousAttempt(clientId, clientSecret);

        var httpRequest = BuildRequest(clientId, clientSecret);

        using var response = await httpClient.SendAsync(httpRequest, ct);
        if (!response.IsSuccessStatusCode)
        {
#if NET8_0_OR_GREATER
            var errorString = await response.Content.ReadAsStringAsync(ct);
#else
            var errorString = await response.Content.ReadAsStringAsync();
#endif

            var exception = new SecurityException($"Failed to retrieve access token for client '{options.ClientId}', got HTTP {response.StatusCode} and error {errorString}.");

            StorePreviousAttempt(clientId, clientSecret, exception);
            throw exception;
        }
#if NET8_0_OR_GREATER
        var jsonString = await response.Content.ReadAsStringAsync(ct);
#else
        var jsonString = await response.Content.ReadAsStringAsync();
#endif
        var jsonToken = JToken.Parse(jsonString);

        return new AuthToken("Authorization", $"Bearer {jsonToken["access_token"]!}");
    }

    private void StorePreviousAttempt(string clientId, string clientSecret, Exception exception)
    {
        invalidAttempts.Set((clientId, clientSecret), exception, options.TokenRetryTime);
    }

    private void ThrowFromPreviousAttempt(string clientId, string clientSecret)
    {
        if (invalidAttempts.TryGet((clientId, clientSecret), out var exception))
        {
            throw exception;
        }
    }

    private static HttpRequestMessage BuildRequest(string clientId, string clientSecret)
    {
        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["scope"] = "squidex-api",
        };

        return new HttpRequestMessage(HttpMethod.Post, TokenUrl)
        {
            Content = new FormUrlEncodedContent(parameters!),
        };
    }
}
