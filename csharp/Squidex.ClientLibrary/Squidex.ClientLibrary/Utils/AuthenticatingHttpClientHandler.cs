// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.ClientLibrary.Utils
{
    public sealed class AuthenticatingHttpClientHandler : HttpClientHandler
    {
        private readonly IAuthenticator authenticator;

        public AuthenticatingHttpClientHandler(IAuthenticator authenticator)
        {
            this.authenticator = authenticator;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await authenticator.GetBearerTokenAsync();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await authenticator.RemoveTokenAsync(token);
            }

            return response;
        }
    }
}
