﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation;

namespace Squidex.CLI.Configuration;

public sealed class GetOnlyHttpMessageHandler : DelegatingHandler
{
    protected override HttpResponseMessage Send(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        EnsureGetOnly(request);
        return base.Send(request, cancellationToken);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        EnsureGetOnly(request);
        return base.SendAsync(request, cancellationToken);
    }

    private static void EnsureGetOnly(HttpRequestMessage request)
    {
        if (request.Method != HttpMethod.Get)
        {
            throw new CLIException("Emulated");
        }
    }
}
