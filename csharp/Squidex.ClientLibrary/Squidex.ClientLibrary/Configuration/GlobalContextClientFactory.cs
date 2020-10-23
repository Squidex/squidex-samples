// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.ClientLibrary.Configuration
{
    public sealed class GlobalContextClientFactory : IHttpClientFactory
    {
        private readonly QueryContext context;

        private sealed class ApplyHeadersHandler : DelegatingHandler
        {
            private readonly QueryContext context;

            public ApplyHeadersHandler(QueryContext context)
            {
                this.context = context;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (context != null)
                {
                    context.AddToHeaders(request.Headers);
                }

                return base.SendAsync(request, cancellationToken);
            }
        }

        public GlobalContextClientFactory(QueryContext context)
        {
            this.context = context;
        }

        public HttpMessageHandler CreateHttpMessageHandler(HttpMessageHandler inner)
        {
            return new ApplyHeadersHandler(context)
            {
                InnerHandler = inner
            };
        }

        public HttpClient CreateHttpClient(HttpMessageHandler messageHandler)
        {
            return null;
        }
    }
}
