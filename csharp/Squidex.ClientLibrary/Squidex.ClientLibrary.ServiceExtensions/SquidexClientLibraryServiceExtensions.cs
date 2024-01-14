// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Squidex.ClientLibrary.ServiceExtensions;
using Squidex.ClientLibrary.Utils;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Service extensions for Squidex Client library.
/// </summary>
public static class SquidexClientLibraryServiceExtensions
{
    /// <summary>
    /// Adds the Squidex client manager to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">A callback to configure Squidex.</param>
    /// <returns>The service collection that was passed in.</returns>
    public static IServiceCollection AddSquidexClient(this IServiceCollection services, Action<SquidexServiceOptions>? configure = null)
    {
        return services.AddSquidexClient(Options.Options.DefaultName, configure);
    }

    /// <summary>
    /// Adds the Squidex client manager to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The name of the client.</param>
    /// <param name="configure">A callback to configure Squidex.</param>
    /// <returns>The service collection that was passed in.</returns>
    public static IServiceCollection AddSquidexClient(this IServiceCollection services, string name, Action<SquidexServiceOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(name, configure);
        }

        services.AddOptions<SquidexServiceOptions>(name)
            .PostConfigure<IHttpClientFactory>((options, c) =>
            {
                var clientName = ClientName(name);

                options.ClientProvider = new HttpClientProvider(() => c.CreateClient(clientName));
            });

        services.TryAddSingleton<IValidateOptions<SquidexServiceOptions>,
            SquidexOptionsValidator>();

        services.TryAddSingleton<ISquidexClientProvider,
            SquidexClientProvider>();

        services.TryAddSingleton(
            c => c.GetRequiredService<ISquidexClientProvider>().Get());

        services.AddSquidexHttpClient(name);

        return services;
    }

    /// <summary>
    /// Adds the Squidex client to the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">The callback to configure the HTTP client.</param>
    /// <returns>The http client builder to make more customizatons.</returns>
    public static IHttpClientBuilder AddSquidexHttpClient(this IServiceCollection services, Action<IServiceProvider, HttpClient>? configure = null)
    {
        return services.AddSquidexHttpClient(Options.Options.DefaultName, configure);
    }

    /// <summary>
    /// Adds the Squidex client to the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="name">The name of the client.</param>
    /// <param name="configure">The callback to configure the HTTP client.</param>
    /// <returns>The http client builder to make more customizatons.</returns>
    public static IHttpClientBuilder AddSquidexHttpClient(this IServiceCollection services, string name, Action<IServiceProvider, HttpClient>? configure = null)
    {
        var builder = services.AddHttpClient(ClientName(name), (c, httpClient) =>
        {
            var options = c.GetRequiredService<IOptionsMonitor<SquidexServiceOptions>>().Get(name);

            if (options.ConfigureHttpClientWithTimeout && options.Timeout != null)
            {
                httpClient.Timeout = options.Timeout.Value;
            }

            if (options.ConfigureHttpClientWithUrl)
            {
                httpClient.BaseAddress = new Uri(options.Url, UriKind.Absolute);
            }

            configure?.Invoke(c, httpClient);
        });

#if NET8_0_OR_GREATER
        builder.ConfigureAdditionalHttpMessageHandlers((handlers, serviceProvider) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<SquidexServiceOptions>>().Value;

            handlers.Add(new AuthenticatingHttpMessageHandler(options));
        });
#else
#pragma warning disable CS0618 // Type or member is obsolete
        builder.ConfigureHttpMessageHandlerBuilder(builder =>
        {
            var options = builder.Services.GetRequiredService<IOptions<SquidexServiceOptions>>().Value;

            if (options.ConfigureHttpClientWithAuthenticator)
            {
                AddSquidexAuthenticatorAsAdditionalHandler(builder);
            }
        });
#pragma warning restore CS0618 // Type or member is obsolete
#endif
        return builder;
    }

    /// <summary>
    /// Adds the Squidex authenticator as additional handler.
    /// </summary>
    /// <param name="builder">The builder to update.</param>
    public static void AddSquidexAuthenticatorAsAdditionalHandler(this HttpMessageHandlerBuilder builder)
    {
        var options = builder.Services.GetRequiredService<IOptions<SquidexServiceOptions>>().Value;

        builder.AdditionalHandlers.Add(new AuthenticatingHttpMessageHandler(options));
    }

    /// <summary>
    /// Sets the Squidex authenticator as primary handler.
    /// </summary>
    /// <param name="builder">The builder to update.</param>
    public static void SetSquidexAuthenticatorAsPrimaryHandler(this HttpMessageHandlerBuilder builder)
    {
        var options = builder.Services.GetRequiredService<IOptions<SquidexServiceOptions>>().Value;

        builder.PrimaryHandler = new AuthenticatingHttpMessageHandler(options);
    }

    internal static string ClientName(string name)
    {
        return $"SquidexHttpClient_{name}";
    }
}
