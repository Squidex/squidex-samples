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

namespace Squidex.ClientLibrary.Utils
{
    /// <summary>
    /// Base class for all clients.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public abstract class SquidexClientBase : IDisposable
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// Gets the name of the App.
        /// </summary>
        /// <value>
        /// The name of the App.
        /// </value>
        protected string ApplicationName
        {
            get { return Options.AppName; }
        }

        /// <summary>
        /// Gets the options of the <see cref="SquidexClientManager"/>.
        /// </summary>
        /// <value>
        /// The options of the <see cref="SquidexClientManager"/>..
        /// </value>
        public SquidexOptions Options { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected internal SquidexClientBase(SquidexOptions options, HttpClient httpClient)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Guard.NotNull(options, nameof(options));
            Guard.NotNull(httpClient, nameof(httpClient));

            Options = options;

            this.httpClient = httpClient;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected internal async Task RequestAsync(HttpMethod method, string path, HttpContent content = null, QueryContext context = null, CancellationToken ct = default)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            using (var request = BuildRequest(method, path, content, context))
            {
                using (var response = await httpClient.SendAsync(request, ct))
                {
                    await EnsureResponseIsValidAsync(response);
                }
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected internal async Task<Stream> RequestStreamAsync(HttpMethod method, string path, HttpContent content = null, QueryContext context = null, CancellationToken ct = default)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            using (var request = BuildRequest(method, path, content, context))
            {
                var response = await httpClient.SendAsync(request, ct);

                await EnsureResponseIsValidAsync(response);

                return await response.Content.ReadAsStreamAsync();
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected internal async Task<T> RequestJsonAsync<T>(HttpMethod method, string path, HttpContent content = null, QueryContext context = null, CancellationToken ct = default)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected internal static HttpRequestMessage BuildRequest(HttpMethod method, string path, HttpContent content, QueryContext context = null)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            var request = new HttpRequestMessage(method, path);

            if (content != null)
            {
                request.Content = content;
            }

            context?.AddToHeaders(request.Headers);

            return request;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected internal async Task EnsureResponseIsValidAsync(HttpResponseMessage response)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
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

        /// <inheritdoc/>
        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
