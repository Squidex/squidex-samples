// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Concurrent;
using System.Net;

namespace Squidex.ClientLibrary.Tests;

internal sealed class SampleLoggingHandler : DelegatingHandler
{
    public ConcurrentBag<(string Url, bool IsAuthorized, HttpStatusCode StatusCode)> Log { get; } = new ();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        Log.Add((
            request.RequestUri?.ToString() ?? string.Empty,
            request.Headers.Contains("Authorization"),
            response.StatusCode));

        return response;
    }
}
