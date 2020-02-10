// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;

namespace Squidex.ClientLibrary.Configuration
{
    public sealed class AcceptAllCertificatesConfigurator : IHttpConfigurator
    {
        public static readonly AcceptAllCertificatesConfigurator Instance = new AcceptAllCertificatesConfigurator();

        private AcceptAllCertificatesConfigurator()
        {
        }

        public void Configure(HttpClient httpClient)
        {
        }

        public void Configure(HttpClientHandler httpClientHandler)
        {
            httpClientHandler.ServerCertificateCustomValidationCallback += (message, certificate, chain, errors) => true;
        }
    }
}
