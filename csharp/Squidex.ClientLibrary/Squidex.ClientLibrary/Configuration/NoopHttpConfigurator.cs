// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;

namespace Squidex.ClientLibrary.Configuration
{
    /// <summary>
    /// Default implementation of the <see cref="IHttpClientFactory"/> and <see cref="IHttpConfigurator"/> that does
    /// not do anything and provides the default behavior.
    /// </summary>
    /// <seealso cref="IHttpConfigurator" />
    /// <seealso cref="IHttpClientFactory" />
    public sealed class NoopHttpConfigurator : IHttpConfigurator, IHttpClientFactory
    {
        /// <summary>
        /// The only instance of this class.
        /// </summary>
        public static readonly NoopHttpConfigurator Instance = new NoopHttpConfigurator();

        private NoopHttpConfigurator()
        {
        }

        /// <inheritdoc/>
        public void Configure(HttpClient httpClient)
        {
        }

        /// <inheritdoc/>
        public void Configure(HttpClientHandler httpClientHandler)
        {
        }

        /// <inheritdoc/>
        public HttpClient? CreateHttpClient(HttpMessageHandler messageHandler)
        {
            return null;
        }

        /// <inheritdoc/>
        public HttpMessageHandler? CreateHttpMessageHandler(HttpMessageHandler inner)
        {
            return inner;
        }
    }
}
