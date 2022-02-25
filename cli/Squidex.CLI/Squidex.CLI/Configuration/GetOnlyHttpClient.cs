// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation;

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
                throw new CLIException("Emulated");
            }

            return base.Send(request, cancellationToken);
        }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Get)
            {
                throw new CLIException("Emulated");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
