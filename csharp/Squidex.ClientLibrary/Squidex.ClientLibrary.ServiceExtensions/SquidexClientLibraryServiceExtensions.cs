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
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.ServiceExtensions;
using Squidex.ClientLibrary.Utils;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service extensions for Squidex Client library.
    /// </summary>
    public static class SquidexClientLibraryServiceExtensions
    {
        /// <summary>
        /// The name of the HTTP client.
        /// </summary>
        public const string ClientName = "SquidexClient";

        /// <summary>
        /// Adds the Squidex client manager to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">A callback to configure Squidex.</param>
        /// <returns>The service collection that was passed in.</returns>
        public static IServiceCollection AddSquidexClient(this IServiceCollection services, Action<SquidexServiceOptions>? configure)
        {
            if (configure != null)
            {
                services.Configure(configure);
            }

            services.AddOptions<SquidexServiceOptions>()
                .PostConfigure<IServiceProvider>((options, c) =>
                {
                    options.ClientProvider = new HttpClientProvider(c.GetRequiredService<IHttpClientFactory>(), ClientName);
                });

            services.TryAddSingleton<IValidateOptions<SquidexServiceOptions>,
                  OptionsValidator>();

            AddSquidexHttpClient(services, null);

            services.TryAddSingleton<ISquidexClientManager>(
                c => new SquidexClientManager(c.GetRequiredService<IOptions<SquidexServiceOptions>>().Value));

            services.AddSquidexClient(m => m.CreateAppsClient());
            services.AddSquidexClient(m => m.CreateAssetsClient());
            services.AddSquidexClient(m => m.CreateBackupsClient());
            services.AddSquidexClient(m => m.CreateCommentsClient());
            services.AddSquidexClient(m => m.CreateDiagnosticsClient());
            services.AddSquidexClient(m => m.CreateEventConsumersClient());
            services.AddSquidexClient(m => m.CreateExtendableRulesClient());
            services.AddSquidexClient(m => m.CreateCommentsClient());
            services.AddSquidexClient(m => m.CreateHistoryClient());
            services.AddSquidexClient(m => m.CreateLanguagesClient());
            services.AddSquidexClient(m => m.CreatePingClient());
            services.AddSquidexClient(m => m.CreatePlansClient());
            services.AddSquidexClient(m => m.CreateRulesClient());
            services.AddSquidexClient(m => m.CreateSchemasClient());
            services.AddSquidexClient(m => m.CreateSearchClient());
            services.AddSquidexClient(m => m.CreateStatisticsClient());
            services.AddSquidexClient(m => m.CreateTeamsClient());
            services.AddSquidexClient(m => m.CreateTemplatesClient());
            services.AddSquidexClient(m => m.CreateTranslationsClient());
            services.AddSquidexClient(m => m.CreateUserManagementClient());
            services.AddSquidexClient(m => m.CreateUsersClient());

            return services;
        }

        /// <summary>
        /// Adds the Squidex client to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="provider">A callback to provider the client from the manager.</param>
        /// <returns>The service collection that was passed in.</returns>
        /// <typeparam name="TClient">The type of the client to register.</typeparam>
        public static IServiceCollection AddSquidexClient<TClient>(this IServiceCollection services, Func<ISquidexClientManager, TClient> provider) where TClient : class
        {
            services.TryAddSingleton(c => provider(c.GetRequiredService<ISquidexClientManager>()));

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
            return services.AddHttpClient(ClientName, (c, httpClient) =>
            {
                var options = c.GetRequiredService<IOptions<SquidexServiceOptions>>().Value;

                if (options.ConfigureHttpClientWithTimeout)
                {
                    httpClient.Timeout = options.HttpClientTimeout;
                }

                if (options.ConfigureHttpClientWithUrl)
                {
                    httpClient.BaseAddress = new Uri(options.Url, UriKind.Absolute);
                }

                configure?.Invoke(c, httpClient);
            }).ConfigureHttpMessageHandlerBuilder(builder =>
            {
                var options = builder.Services.GetRequiredService<IOptions<SquidexServiceOptions>>().Value;

                if (options.ConfigureHttpClientWithAuthenticator)
                {
                    AddSquidexAuthenticatorAsAdditionalHandler(builder);
                }
            });
        }

        /// <summary>
        /// Adds the Squidex authenticator as additional handler.
        /// </summary>
        /// <param name="builder">The builder to update.</param>
        public static void AddSquidexAuthenticatorAsAdditionalHandler(this HttpMessageHandlerBuilder builder)
        {
            var options = builder.Services.GetRequiredService<IOptions<SquidexServiceOptions>>().Value;

            builder.AdditionalHandlers.Add(new AuthenticatingHttpMessageHandler(options.Authenticator));
        }

        /// <summary>
        /// Sets the Squidex authenticator as primary handler.
        /// </summary>
        /// <param name="builder">The builder to update.</param>
        public static void SetSquidexAuthenticatorAsPrimaryHandler(this HttpMessageHandlerBuilder builder)
        {
            var options = builder.Services.GetRequiredService<IOptions<SquidexServiceOptions>>().Value;

            builder.PrimaryHandler = new AuthenticatingHttpMessageHandler(options.Authenticator);
        }
    }
}
