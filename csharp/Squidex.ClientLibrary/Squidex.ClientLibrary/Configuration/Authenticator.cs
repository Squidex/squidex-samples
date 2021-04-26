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
        private readonly HttpClient httpClient;
        private readonly SquidexOptions squidexOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Authenticator"/> class.
        /// </summary>
        /// <param name="squidexOptions">The options to configure.</param>
        /// <exception cref="ArgumentNullException"><paramref name="squidexOptions"/> is null.</exception>
        public Authenticator(SquidexOptions squidexOptions)
        {
            Guard.NotNull(squidexOptions, nameof(squidexOptions));

            var handler = new HttpClientHandler();

            squidexOptions.Configurator.Configure(handler);

            httpClient =
                squidexOptions.ClientFactory.CreateHttpClient(handler) ??
                new HttpClient(handler, false);

            httpClient.Timeout = squidexOptions.HttpClientTimeout;

            squidexOptions.Configurator.Configure(httpClient);
            this.squidexOptions = squidexOptions;
        }

        /// <inheritdoc/>
        public Task RemoveTokenAsync(string token)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<string> GetBearerTokenAsync()
        {
            var url = $"{squidexOptions.Url}/identity-server/connect/token";

            var bodyString = $"grant_type=client_credentials&client_id={squidexOptions.ClientId}&client_secret={squidexOptions.ClientSecret}&scope=squidex-api";
            var bodyContent = new StringContent(bodyString, Encoding.UTF8, "application/x-www-form-urlencoded");

            using (var response = await httpClient.PostAsync(url, bodyContent))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new SecurityException($"Failed to retrieve access token for client '{squidexOptions.ClientId}', got HTTP {response.StatusCode}.");
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonToken = JToken.Parse(jsonString);

                return jsonToken["access_token"].ToString();
            }
        }
    }
}
