// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.DependencyInjection;
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

            AddSquidexHttpClient(services, null);

            services.AddSingleton<ISquidexClientManager>(c =>
            {
                var options = c.GetRequiredService<IOptions<SquidexServiceOptions>>().Value;

                options.ClientProvider = new HttpClientProvider(c.GetRequiredService<IHttpClientFactory>(), ClientName);

                return new SquidexClientManager(options);
            });

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateAppsClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateAssetsClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateBackupsClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateCommentsClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateDiagnosticsClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateEventConsumersClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateExtendableRulesClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateCommentsClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateHistoryClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateLanguagesClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreatePingClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreatePlansClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateRulesClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateSchemasClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateSearchClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateStatisticsClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateTeamsClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateTemplatesClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateTranslationsClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateUserManagementClient());

            services.AddSingleton(
                c => c.GetRequiredService<ISquidexClientManager>().CreateUsersClient());

            return services;
        }

        /// <summary>
        /// Adds the Squidex client to the service collection.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="configure">The callback to configure the HTTP client.</param>
        /// <returns>The http client builder to make more customizatons.</returns>
        public static IHttpClientBuilder AddSquidexHttpClient(IServiceCollection services, Action<IServiceProvider, HttpClient>? configure = null)
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
                    AddSqquidexAuthenticatorAsAdditionalHandler(builder);
                }
            });
        }

        /// <summary>
        /// Adds the Squidex authenticator as additional handler.
        /// </summary>
        /// <param name="builder">The builder to update.</param>
        public static void AddSqquidexAuthenticatorAsAdditionalHandler(this HttpMessageHandlerBuilder builder)
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
