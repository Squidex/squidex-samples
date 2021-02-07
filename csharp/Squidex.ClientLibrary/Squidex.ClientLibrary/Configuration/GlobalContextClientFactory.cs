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
    /// <summary>
    /// A <see cref="IHttpClientFactory"/> that adds the context options to all requests.
    /// </summary>
    /// <seealso cref="IHttpClientFactory" />
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
                context?.AddToHeaders(request.Headers);

                return base.SendAsync(request, cancellationToken);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalContextClientFactory"/> class with the context to apply.
        /// </summary>
        /// <param name="context">The context to apply.</param>
        public GlobalContextClientFactory(QueryContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public HttpMessageHandler CreateHttpMessageHandler(HttpMessageHandler inner)
        {
            return new ApplyHeadersHandler(context)
            {
                InnerHandler = inner
            };
        }

        /// <inheritdoc/>
        public HttpClient CreateHttpClient(HttpMessageHandler messageHandler)
        {
            return null;
        }
    }
}
