// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.Configuration
{
    /// <summary>
    /// Configures the HTTP client to accept all SSL certificates.
    /// </summary>
    /// <seealso cref="IHttpConfigurator" />
    public sealed class AcceptAllCertificatesConfigurator : IHttpConfigurator
    {
        /// <summary>
        /// The only instance of this class.
        /// </summary>
        public static readonly AcceptAllCertificatesConfigurator Instance = new AcceptAllCertificatesConfigurator();

        private AcceptAllCertificatesConfigurator()
        {
        }

        /// <inheritdoc/>
        public void Configure(HttpClient httpClient)
        {
        }

        /// <inheritdoc/>
        public void Configure(HttpClientHandler httpClientHandler)
        {
            httpClientHandler.ServerCertificateCustomValidationCallback += (message, certificate, chain, errors) => true;
        }
    }
}
