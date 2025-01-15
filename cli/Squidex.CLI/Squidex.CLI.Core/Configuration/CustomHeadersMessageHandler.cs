// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Configuration;

public sealed class CustomHeadersMessageHandler(Dictionary<string, string> headers) : DelegatingHandler
{
    protected override HttpResponseMessage Send(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        AddHeaders(request);
        return base.Send(request, cancellationToken);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        AddHeaders(request);
        return base.SendAsync(request, cancellationToken);
    }

    private void AddHeaders(HttpRequestMessage request)
    {
        foreach (var (key, value) in headers)
        {
            request.Headers.TryAddWithoutValidation(key, value);
        }
    }
}
