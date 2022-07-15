// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Management;

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
        public string AppName { get; }

        /// <summary>
        /// Gets the options of the <see cref="SquidexClientManager"/>.
        /// </summary>
        /// <value>
        /// The options of the <see cref="SquidexClientManager"/>.
        /// </value>
        public SquidexOptions Options { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected internal SquidexClientBase(SquidexOptions options, string appName, HttpClient httpClient)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Guard.NotNull(options, nameof(options));
            Guard.NotNullOrEmpty(appName, nameof(appName));
            Guard.NotNull(httpClient, nameof(httpClient));

            // The app name can be different from the options app name.
            AppName = appName;

            Options = options;

            this.httpClient = httpClient;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected internal async Task RequestAsync(HttpMethod method, string path, HttpContent? content, QueryContext? context, CancellationToken ct)
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
        protected internal async Task<Stream> RequestStreamAsync(HttpMethod method, string path, HttpContent? content, QueryContext? context, CancellationToken ct)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            using (var request = BuildRequest(method, path, content, context))
            {
                var response = await httpClient.SendAsync(request, ct);

                await EnsureResponseIsValidAsync(response);
#if NET5_0_OR_GREATER
                return await response.Content.ReadAsStreamAsync(ct);
#else
                return await response.Content.ReadAsStreamAsync();
#endif
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected internal async Task<T> RequestJsonAsync<T>(HttpMethod method, string path, HttpContent? content, QueryContext? context, CancellationToken ct)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            using (var request = BuildRequest(method, path, content, context))
            {
                using (var response = await httpClient.SendAsync(request, ct))
                {
                    await EnsureResponseIsValidAsync(response);

                    return (await response.Content.ReadAsJsonAsync<T>())!;
                }
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected internal static HttpRequestMessage BuildRequest(HttpMethod method, string path, HttpContent? content, QueryContext? context)
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
        protected internal static async Task EnsureResponseIsValidAsync(HttpResponseMessage response)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;

                if (statusCode == 404)
                {
                    throw new SquidexException("The app, schema or entity does not exist.", statusCode, null);
                }

                if (statusCode == 429)
                {
                    throw new SquidexException("Too many requests, please upgrade your subscription.", statusCode, null);
                }

                var message = await response.Content.ReadAsStringAsync();

                ErrorDto? details = null;

                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "Squidex API failed with internal error.";
                }
                else
                {
                    try
                    {
                        details = message.FromJson<ErrorDto>();
                    }
                    catch
                    {
                        details = null;
                    }

                    if (!string.IsNullOrWhiteSpace(details?.Message))
                    {
                        message = $"Squidex Request failed: {details!.Message}";
                    }
                    else
                    {
                        message = $"Squidex Request failed: {message}";
                    }
                }

                throw new SquidexException(message, statusCode, details);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            httpClient?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
