﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net;
using System.Security;
using System.Text;
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
        private const string AuthenticationHeader = "X-AuthRequest";
        private readonly SquidexOptions options;

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
            return !request.Headers.Contains(AuthenticationHeader);
        }

        /// <inheritdoc/>
        public Task RemoveTokenAsync(string token,
            CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<string> GetBearerTokenAsync(
            CancellationToken ct)
        {
            var httpClient = options.ClientProvider.Get();
            try
            {
                var httpRequest = BuildRequest();

                using (var response = await httpClient.SendAsync(httpRequest, ct))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new SecurityException($"Failed to retrieve access token for client '{options.ClientId}', got HTTP {response.StatusCode}.");
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

        private HttpRequestMessage BuildRequest()
        {
            const string url = "identity-server/connect/token";

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = options.ClientId,
                ["client_secret"] = options.ClientSecret,
                ["scope"] = "squidex-api"
            });

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            httpRequest.Headers.TryAddWithoutValidation(AuthenticationHeader, "1");

            return httpRequest;
        }
    }
}
