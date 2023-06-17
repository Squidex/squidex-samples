// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.Utils;

/// <summary>
/// Base class for all clients.
/// </summary>
/// <seealso cref="IDisposable" />
public abstract class SquidexClientBase
{
    /// <summary>
    /// Gets the options of the <see cref="SquidexClient"/>.
    /// </summary>
    /// <value>
    /// The options of the <see cref="SquidexClient"/>.
    /// </value>
    public SquidexOptions Options { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    protected internal SquidexClientBase(SquidexOptions options)
    {
        Guard.NotNull(options, nameof(options));

        // Just pass in the options to have direct access to them.
        Options = options;
    }

    protected internal async Task RequestAsync(HttpMethod method, string path, HttpContent? content, QueryContext? context,
        CancellationToken ct)
    {
        var httpClient = Options.ClientProvider.Get();

        using (var request = BuildRequest(method, path, content, context))
        {
            using (var response = await httpClient.SendAsync(request, ct))
            {
                await EnsureResponseIsValidAsync(response);
            }
        }
    }

    protected internal async Task<Stream> RequestStreamAsync(HttpMethod method, string path, HttpContent? content, QueryContext? context,
        CancellationToken ct)
    {
        var httpClient = Options.ClientProvider.Get();

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

    protected internal async Task<T> RequestJsonAsync<T>(HttpMethod method, string path, HttpContent? content, QueryContext? context,
        CancellationToken ct)
    {
        var httpClient = Options.ClientProvider.Get();

        using (var request = BuildRequest(method, path, content, context))
        {
            using (var response = await httpClient.SendAsync(request, ct))
            {
                await EnsureResponseIsValidAsync(response);

                return (await response.Content.ReadAsJsonAsync<T>())!;
            }
        }
    }

    protected internal HttpRequestMessage BuildRequest(HttpMethod method, string path, HttpContent? content, QueryContext? context)
    {
        var request = new HttpRequestMessage(method, path);

        if (content != null)
        {
            request.Content = content;
        }

        context?.AddToHeaders(request.Headers);

        return request;
    }

    protected internal static async Task EnsureResponseIsValidAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var statusCode = (int)response.StatusCode;

            if (statusCode == 404)
            {
                throw new SquidexException("The app, schema or entity does not exist.", statusCode);
            }

            if (statusCode == 429)
            {
                throw new SquidexException("Too many requests, please upgrade your subscription.", statusCode);
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

            throw new SquidexException<ErrorDto>(message, statusCode, details);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
