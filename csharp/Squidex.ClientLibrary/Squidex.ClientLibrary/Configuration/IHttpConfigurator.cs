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
    /// Implement this interface to change the behavior of HTTP requests.
    /// </summary>
    public interface IHttpConfigurator
    {
        /// <summary>
        /// Configures the specified HTTP client.
        /// </summary>
        /// <param name="httpClient">The HTTP client to configure.</param>
        void Configure(HttpClient httpClient);

        /// <summary>
        /// Configures the specified HTTP client handler.
        /// </summary>
        /// <param name="httpClientHandler">The HTTP client handler to configure.</param>
        void Configure(HttpClientHandler httpClientHandler);
    }
}
