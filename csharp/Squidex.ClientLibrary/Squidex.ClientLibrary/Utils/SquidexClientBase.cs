// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable IDE0067 // Dispose objects before losing scope

namespace Squidex.ClientLibrary.Utils
{
    public abstract class SquidexClientBase : IDisposable
    {
        private readonly HttpClient httpClient;

        protected string ApplicationName
        {
            get { return Options.AppName; }
        }

        public SquidexOptions Options { get; }

        protected SquidexClientBase(SquidexOptions options, HttpClient httpClient)
        {
            Guard.NotNull(options, nameof(options));
            Guard.NotNull(httpClient, nameof(httpClient));

            Options = options;

            this.httpClient = httpClient;
        }

        protected async Task RequestAsync(HttpMethod method, string path, HttpContent content = null, QueryContext context = null, CancellationToken ct = default)
        {
            using (var request = BuildRequest(method, path, content, context))
            {
                using (var response = await httpClient.SendAsync(request, ct))
                {
                    await EnsureResponseIsValidAsync(response);
                }
            }
        }

        protected async Task<Stream> RequestStreamAsync(HttpMethod method, string path, HttpContent content = null, QueryContext context = null, CancellationToken ct = default)
        {
            using (var request = BuildRequest(method, path, content, context))
            {
                var response = await httpClient.SendAsync(request, ct);

                await EnsureResponseIsValidAsync(response);

                return await response.Content.ReadAsStreamAsync();
            }
        }

        protected async Task<T> RequestJsonAsync<T>(HttpMethod method, string path, HttpContent content = null, QueryContext context = null, CancellationToken ct = default)
        {
            using (var request = BuildRequest(method, path, content, context))
            {
                using (var response = await httpClient.SendAsync(request, ct))
                {
                    await EnsureResponseIsValidAsync(response);

                    return await response.Content.ReadAsJsonAsync<T>();
                }
            }
        }

        protected static HttpRequestMessage BuildRequest(HttpMethod method, string path, HttpContent content, QueryContext context = null)
        {
            var request = new HttpRequestMessage(method, path);

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
            httpClient?.Dispose();
        }
    }
}
