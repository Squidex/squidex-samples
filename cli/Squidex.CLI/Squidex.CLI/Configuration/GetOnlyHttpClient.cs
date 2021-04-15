// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.CLI.Configuration
{
    public sealed class GetOnlyHttpClient : HttpClient
    {
        public GetOnlyHttpClient(HttpMessageHandler messageHandler)
            : base(messageHandler)
        {
        }

        public override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Get)
            {
                throw new InvalidOperationException("Not possible in emulation mode.");
            }

            return base.Send(request, cancellationToken);
        }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Get)
            {
                throw new InvalidOperationException("Not possible in emulation mode.");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
