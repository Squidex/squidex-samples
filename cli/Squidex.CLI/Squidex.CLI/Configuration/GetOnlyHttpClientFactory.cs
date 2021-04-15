// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;
using Squidex.ClientLibrary.Configuration;

namespace Squidex.CLI.Configuration
{
    public sealed class GetOnlyHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateHttpClient(HttpMessageHandler messageHandler)
        {
            return new GetOnlyHttpClient(messageHandler);
        }

        public HttpMessageHandler CreateHttpMessageHandler(HttpMessageHandler inner)
        {
            return null;
        }
    }
}
