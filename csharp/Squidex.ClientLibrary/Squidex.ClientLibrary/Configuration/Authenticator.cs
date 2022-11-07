// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Security;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.Configuration
{
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
        public Task RemoveTokenAsync(string appName, string token,
            CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<string> GetBearerTokenAsync(string appName,
            CancellationToken ct)
        {
            var httpClient = options.ClientProvider.Get();
            try
            {
                var clientId = options.ClientId;
                var clientSecret = options.ClientSecret;

                if (options.AppCredentials != null && options.AppCredentials.TryGetValue(appName, out var credentials))
                {
                    clientId = credentials.ClientId;
                    clientSecret = credentials.ClientSecret;
                }

                ThrowFromPreviousAttempt(clientId, clientSecret);

                var httpRequest = BuildRequest(clientId, clientSecret);

                using (var response = await httpClient.SendAsync(httpRequest, ct))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var exception = new SecurityException($"Failed to retrieve access token for client '{options.ClientId}', got HTTP {response.StatusCode}.");

                        StorePreviousAttempt(clientId, clientSecret, exception);
                        throw exception;
                    }
#if NET5_0_OR_GREATER
                    var jsonString = await response.Content.ReadAsStringAsync(ct);
#else
                    var jsonString = await response.Content.ReadAsStringAsync();
#endif
                    var jsonToken = JToken.Parse(jsonString);

                    return jsonToken["access_token"]!.ToString();
                }
            }
            finally
            {
                options.ClientProvider.Return(httpClient);
            }
        }

        private void StorePreviousAttempt(string clientId, string clientSecret, Exception exception)
        {
            var retryTime = options.TokenRetryTime;

            if (retryTime < TimeSpan.Zero || retryTime == TimeSpan.MaxValue)
            {
                return;
            }

            invalidAttempts.Set((clientId, clientSecret), exception, options.TokenRetryTime);
        }

        private void ThrowFromPreviousAttempt(string clientId, string clientSecret)
        {
            var retryTime = options.TokenRetryTime;

            if (retryTime < TimeSpan.Zero || retryTime == TimeSpan.MaxValue)
            {
                return;
            }

            if (invalidAttempts.TryGet((clientId, clientSecret), out var exception))
            {
                throw exception;
            }
        }

        private HttpRequestMessage BuildRequest(string clientId, string clientSecret)
        {
            var parameters = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["scope"] = "squidex-api"
            };

            return new HttpRequestMessage(HttpMethod.Post, TokenUrl)
            {
                Content = new FormUrlEncodedContent(parameters!)
            };
        }
    }
}
