// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;

namespace Squidex.ClientLibrary.Configuration
{
    public interface IHttpClientFactory
    {
        HttpClient CreateHttpClient();

        HttpClientHandler CreateHttpClientHandler(HttpClientHandler inner);
    }
}
