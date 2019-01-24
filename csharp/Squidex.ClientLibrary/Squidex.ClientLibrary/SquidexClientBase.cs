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

using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary
{
    public abstract class SquidexClientBase : IDisposable
    {
        protected HttpClient HttpClient { get; }

        protected Uri ServiceUrl { get; }

        protected string ApplicationName { get; }

        protected SquidexClientBase(Uri serviceUrl, string applicationName, IAuthenticator authenticator)
        {
            Guard.NotNull(serviceUrl, nameof(serviceUrl));
            Guard.NotNullOrEmpty(applicationName, nameof(applicationName));

            HttpClient = new HttpClient(new AuthenticatingHttpClientHandler(authenticator), true);
            ApplicationName = applicationName;
            ServiceUrl = serviceUrl;
        }

        protected async Task<HttpResponseMessage> RequestAsync(HttpMethod method, string path, HttpContent content = null, QueryContext context = null)
        {
            var uri = new Uri(ServiceUrl, path);

            using (var request = BuildRequest(method, uri, content, context))
            {
                var response = await HttpClient.SendAsync(request);

                await EnsureResponseIsValidAsync(response);

                return response;
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

        public void Dispose()
        {
            this.HttpClient?.Dispose();
        }
    }
}
