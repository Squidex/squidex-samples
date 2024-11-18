// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Configuration;

namespace Squidex.ClientLibrary.ServiceExtensions;

internal sealed class HttpClientProvider(Func<HttpClient> factory) : IHttpClientProvider
{
    /// <inheritdoc />
    public HttpClient Get()
    {
        return factory();
    }
}
