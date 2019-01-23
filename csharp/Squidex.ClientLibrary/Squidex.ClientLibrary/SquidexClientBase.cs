// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Squidex.ClientLibrary
{
    public abstract class SquidexClientBase
    {
        protected Func<HttpClient> HttpClientFactory { get; }

        protected Uri ServiceUrl { get; }

        protected string ApplicationName { get; }

        protected SquidexClientBase(Uri serviceUrl, string applicationName, string schemaName, Func<HttpClient> httpClientFactory)
        {
            Guard.NotNull(serviceUrl, nameof(serviceUrl));
            Guard.NotNull(httpClientFactory, nameof(httpClientFactory));
            Guard.NotNullOrEmpty(applicationName, nameof(applicationName));

            HttpClientFactory = httpClientFactory;
            ApplicationName = applicationName;
            ServiceUrl = serviceUrl;
        }

        protected async Task<HttpResponseMessage> RequestAsync(HttpMethod method, string path, HttpContent content = null, QueryContext context = null)
        {
            using (var httpClient = HttpClientFactory())
            {
                var uri = new Uri(ServiceUrl, path);

                using (var request = BuildRequest(method, uri, content, context))
                {
                    var response = await httpClient.SendAsync(request);

                    await EnsureResponseIsValidAsync(response);

                    return response;
                }
            }
        }

        protected static HttpRequestMessage BuildRequest(HttpMethod method, Uri uri, HttpContent content, QueryContext context = null)
        {
            var request = new HttpRequestMessage(method, uri);

            if (content != null)
            {
                request.Content = content;
            }

            if (context != null)
            {
                context.AddToHeaders(request.Headers);
            }

            return request;
        }

        protected async Task EnsureResponseIsValidAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new SquidexException("The app, schema or entity does not exist.");
                }

                if ((int)response.StatusCode == 429)
                {
                    throw new SquidexException("Too many requests, please upgrade your subscription.");
                }

                var message = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "Squidex API failed with internal error.";
                }
                else
                {
                    message = $"Squidex Request failed: {message}";
                }

                throw new SquidexException(message);
            }
        }
    }
}
