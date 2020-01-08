// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;

namespace Squidex.ClientLibrary
{
    public sealed class NoopHttpConfigurator : IHttpConfigurator
    {
        public static readonly NoopHttpConfigurator Instance = new NoopHttpConfigurator();

        private NoopHttpConfigurator()
        {
        }

        public void Configure(HttpClient httpClient)
        {
        }

        public void Configure(HttpClientHandler httpClientHandler)
        {
        }
    }
}
