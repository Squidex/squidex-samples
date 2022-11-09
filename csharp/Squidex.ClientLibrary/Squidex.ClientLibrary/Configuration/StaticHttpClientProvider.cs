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
public sealed class StaticHttpClientProvider : IHttpClientProvider
{
    private readonly HttpClient staticHttpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticHttpClientProvider"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public StaticHttpClientProvider(SquidexOptions options)
    {
        Guard.NotNull(options, nameof(options));

        staticHttpClient = CreateClient(options);
    }

    private static HttpClient CreateClient(SquidexOptions options)
    {
        var handler = new HttpClientHandler();

        options.Configurator.Configure(handler);

        HttpMessageHandler messageHandler = new AuthenticatingHttpMessageHandler(options.Authenticator)
        {
            InnerHandler = handler
        };

        messageHandler = options.ClientFactory.CreateHttpMessageHandler(messageHandler) ?? messageHandler;

        var httpClient =
            options.ClientFactory.CreateHttpClient(messageHandler) ??
                new HttpClient(messageHandler, false);

        // Apply this setting afterwards, to override the value from the client factory.
        httpClient.BaseAddress = new Uri(options.Url, UriKind.Absolute);

        // Also override timeout when create from factory.
        httpClient.Timeout = options.HttpClientTimeout;

        options.Configurator.Configure(httpClient);

        return httpClient;
    }

    /// <inheritdoc />
    public HttpClient Get()
    {
        return staticHttpClient;
    }

    /// <inheritdoc />
    public void Return(HttpClient httpClient)
    {
    }
}
