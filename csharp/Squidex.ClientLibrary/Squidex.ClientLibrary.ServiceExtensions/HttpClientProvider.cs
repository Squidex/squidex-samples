// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Configuration;

namespace Squidex.ClientLibrary.ServiceExtensions
{
    internal class HttpClientProvider : IHttpClientProvider
    {
        private readonly Func<HttpClient> factory;

        public HttpClientProvider(System.Net.Http.IHttpClientFactory httpClientFactory, string name)
        {
            factory = () =>
            {
                return httpClientFactory.CreateClient(name);
            };
        }

        public HttpClient Get()
        {
            return factory();
        }

        public void Return(HttpClient httpClient)
        {
            httpClient.Dispose();
        }
    }
}
