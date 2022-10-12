// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Configuration;

namespace Squidex.ClientLibrary.ServiceExtensions
{
    /// <summary>
    /// Default http client provider, that works as wrapper over <see cref="System.Net.Http.IHttpClientFactory"/>.
    /// </summary>
    public sealed class HttpClientProvider : IHttpClientProvider
    {
        private readonly Func<HttpClient> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientProvider"/> class with the wrapped  <see cref="System.Net.Http.IHttpClientFactory"/> see.
        /// </summary>
        /// <param name="httpClientFactory">The wrapped  <see cref="System.Net.Http.IHttpClientFactory"/>. Cannot be null.</param>
        /// <param name="httpClientName">The logical name of the client to create. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="httpClientFactory"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="httpClientName"/> is null.</exception>
        public HttpClientProvider(System.Net.Http.IHttpClientFactory httpClientFactory, string httpClientName)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            if (httpClientName == null)
            {
                throw new ArgumentNullException(nameof(httpClientName));
            }

            factory = () =>
            {
                return httpClientFactory.CreateClient(httpClientName);
            };
        }

        /// <inheritdoc />
        public HttpClient Get()
        {
            return factory();
        }

        /// <inheritdoc />
        public void Return(HttpClient httpClient)
        {
            httpClient.Dispose();
        }
    }
}
