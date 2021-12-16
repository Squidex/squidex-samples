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
    /// Optional interface to create new <see cref="HttpClient"/> instances.
    /// </summary>
    /// <remarks>
    /// Implement this class if you have custom requirements how the HTTP requests need to be done.
    /// </remarks>
    public interface IHttpClientFactory
    {
        /// <summary>
        /// Creates the HTTP client from the message.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <returns>
        /// The HTTP client.
        /// </returns>
        HttpClient? CreateHttpClient(HttpMessageHandler messageHandler);

        /// <summary>
        /// Creates the HTTP message handler from the inner handler.
        /// </summary>
        /// <param name="inner">The inner handler.</param>
        /// <returns>
        /// The HTTP message handler.
        /// </returns>
        HttpMessageHandler? CreateHttpMessageHandler(HttpMessageHandler inner);
    }
}
