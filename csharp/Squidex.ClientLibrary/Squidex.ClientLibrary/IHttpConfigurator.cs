// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;

namespace Squidex.ClientLibrary
{
    public interface IHttpConfigurator
    {
        void Configure(HttpClient httpClient);

        void Configure(HttpClientHandler httpClientHandler);
    }
}
