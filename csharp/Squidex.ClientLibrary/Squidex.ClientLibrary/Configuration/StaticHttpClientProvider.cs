// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.Configuration;

/// <summary>
/// Provides a static instance.
/// </summary>
public class StaticHttpClientProvider : IHttpClientProvider
{
    private readonly SquidexOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticHttpClientProvider"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public StaticHttpClientProvider(SquidexOptions options)
    {
        Guard.NotNull(options, nameof(options));

        this.options = options;
    }

    /// <summary>
    /// Creates the client from the options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>
    /// The created HTTP client.
    /// </returns>
    protected virtual HttpClient CreateHttpClient(SquidexOptions options)
    {
        var messageHandler = CreateMessageHandler(options);

        var httpClient = new HttpClient(messageHandler, false)
        {
            BaseAddress = new Uri(options.Url, UriKind.Absolute)
        };

        if (options.Timeout != null)
        {
            httpClient.Timeout = options.Timeout.Value;
        }

        return httpClient;
    }

    /// <summary>
    /// Creates the client handler from the options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>
    /// The created HTTP client handler.
    /// </returns>
    protected virtual HttpMessageHandler CreateMessageHandler(SquidexOptions options)
    {
        var innerHandler = new HttpClientHandler();

        if (options.IgnoreSelfSignedCertificates)
        {
            innerHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
        }

        var messageHandler = new AuthenticatingHttpMessageHandler(options)
        {
            InnerHandler = innerHandler
        };

        return messageHandler;
    }

    /// <inheritdoc />
    public HttpClient Get()
    {
        return staticHttpClient ?? CreateHttpClient(options);
    }
}
