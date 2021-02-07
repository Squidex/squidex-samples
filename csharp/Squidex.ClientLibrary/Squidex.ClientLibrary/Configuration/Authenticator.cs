// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
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
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly Uri serviceUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="Authenticator"/> class with the service URL, client identifier and secret.
        /// </summary>
        /// <param name="serviceUrl">The service URL. Cannot be null or empty.</param>
        /// <param name="clientId">The client identifier. Cannot be null or empty.</param>
        /// <param name="clientSecret">The client secret. Cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUrl"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="clientId"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="clientSecret"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="serviceUrl"/> is empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="clientId"/> is empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="clientSecret"/> is empty.</exception>
        public Authenticator(string serviceUrl, string clientId, string clientSecret)
            : this(new Uri(serviceUrl, UriKind.Absolute), clientId, clientSecret)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Authenticator"/> class with the service URL, client identifier and secret.
        /// </summary>
        /// <param name="serviceUrl">The service URL. Cannot be null.</param>
        /// <param name="clientId">The client identifier. Cannot be null or empty.</param>
        /// <param name="clientSecret">The client secret. Cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceUrl"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="clientId"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="clientSecret"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="clientId"/> is empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="clientSecret"/> is empty.</exception>
        public Authenticator(Uri serviceUrl, string clientId, string clientSecret)
        {
            Guard.NotNull(serviceUrl, nameof(serviceUrl));
            Guard.NotNullOrEmpty(clientId, nameof(clientId));
            Guard.NotNullOrEmpty(clientSecret, nameof(clientSecret));

            this.serviceUrl = serviceUrl;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        /// <inheritdoc/>
        public Task RemoveTokenAsync(string token)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<string> GetBearerTokenAsync()
        {
            var url = $"{serviceUrl}identity-server/connect/token";

            var bodyString = $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}&scope=squidex-api";
            var bodyContent = new StringContent(bodyString, Encoding.UTF8, "application/x-www-form-urlencoded");

            using (var response = await httpClient.PostAsync(url, bodyContent))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new SecurityException($"Failed to retrieve access token for client '{clientId}', got HTTP {response.StatusCode}.");
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonToken = JToken.Parse(jsonString);

                return jsonToken["access_token"].ToString();
            }
        }
    }
}
