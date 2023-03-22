// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Configuration;

namespace Squidex.ClientLibrary.ServiceExtensions;

internal sealed class HttpClientProvider : IHttpClientProvider
{
    private readonly Func<HttpClient> factory;

    public HttpClientProvider(Func<HttpClient> factory)
    {
        this.factory = factory;
    }

    /// <inheritdoc />
    public HttpClient Get()
    {
        return factory();
    }
}
