// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Management;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// Default implementation of the <see cref="ISquidexClientManager"/> interface.
    /// </summary>
    /// <seealso cref="ISquidexClientManager" />
    public sealed class SquidexClientManager : ISquidexClientManager
    {
        /// <inheritdoc/>
        public string App
        {
            get { return Options.AppName; }
        }

        /// <inheritdoc/>
        public SquidexOptions Options { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SquidexClientManager"/> class with the options.
        /// </summary>
        /// <param name="options">The options. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        public SquidexClientManager(SquidexOptions options)
        {
            Guard.NotNull(options, nameof(options));

            options.CheckAndFreeze();

            Options = options;
        }

        /// <inheritdoc/>
        public string? GenerateImageUrl(string? id)
        {
            if (id == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(Options.AssetCDN))
            {
                return GenerateUrl(Options.AssetCDN, id);
            }

            return GenerateUrl(Options.Url, $"api/assets/{id}");
        }

        /// <inheritdoc/>
        public string? GenerateImageUrl(IEnumerable<string>? id)
        {
            return GenerateImageUrl(id?.FirstOrDefault());
        }

        /// <inheritdoc />
        public string? GenerateUrl(string? relativeUrl)
        {
            return GenerateUrl(Options.Url, relativeUrl);
        }

        /// <inheritdoc />
        public string? GenerateAssetCDNUrl(string relativeUrl)
        {
            return GenerateUrl(Options.AssetCDN, relativeUrl);
        }

        /// <inheritdoc />
        public string? GenerateContentCDNUrl(string relativeUrl)
        {
            return GenerateUrl(Options.ContentCDN, relativeUrl);
        }

        private string? GenerateUrl(string baseUrl, string? relativeUrl)
        {
            if (relativeUrl == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("URL is not configured.");
            }

            return $"{baseUrl}{relativeUrl.TrimStart('/')}";
        }

        /// <inheritdoc/>
        public IAppsClient CreateAppsClient()
        {
            return new AppsClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IAssetsClient CreateAssetsClient()
        {
            return new AssetsClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IBackupsClient CreateBackupsClient()
        {
            return new BackupsClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ICommentsClient CreateCommentsClient()
        {
            return new CommentsClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IHistoryClient CreateHistoryClient()
        {
            return new HistoryClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ILanguagesClient CreateLanguagesClient()
        {
            return new LanguagesClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IPingClient CreatePingClient()
        {
            return new PingClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IPlansClient CreatePlansClient()
        {
            return new PlansClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IRulesClient CreateRulesClient()
        {
            return new RulesClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ISchemasClient CreateSchemasClient()
        {
            return new SchemasClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IStatisticsClient CreateStatisticsClient()
        {
            return new StatisticsClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ITemplatesClient CreateTemplatesClient()
        {
            return new TemplatesClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IUsersClient CreateUsersClient()
        {
            return new UsersClient(CreateHttpClient(false))
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IExtendableRulesClient CreateExtendableRulesClient()
        {
            return CreateExtendableRulesClient(Options.AppName);
        }

        /// <inheritdoc/>
        public IExtendableRulesClient CreateExtendableRulesClient(string appName)
        {
            return new ExtendableRulesClient(Options, appName, CreateHttpClient(false));
        }

        /// <inheritdoc/>
        public IContentsClient<TEntity, TData> CreateContentsClient<TEntity, TData>(string schemaName) where TEntity : Content<TData> where TData : class, new()
        {
            return CreateContentsClient<TEntity, TData>(Options.AppName, schemaName);
        }

        /// <inheritdoc/>
        public IContentsClient<TEntity, TData> CreateContentsClient<TEntity, TData>(string appName, string schemaName) where TEntity : Content<TData> where TData : class, new()
        {
            return new ContentsClient<TEntity, TData>(Options, appName, schemaName, CreateHttpClient(true));
        }

        /// <inheritdoc/>
        public IContentsClient<DynamicContent, DynamicData> CreateDynamicContentsClient(string schemaName)
        {
            return CreateDynamicContentsClient(Options.AppName, schemaName);
        }

        /// <inheritdoc/>
        public IContentsClient<DynamicContent, DynamicData> CreateDynamicContentsClient(string appName, string schemaName)
        {
            return new ContentsClient<DynamicContent, DynamicData>(Options, appName, schemaName, CreateHttpClient(true));
        }

        /// <inheritdoc/>
        public HttpClient CreateHttpClient(bool appendApi = true)
        {
            var url = new Uri(Options.Url, UriKind.Absolute);

            if (appendApi)
            {
                url = new Uri(url, "api/");
            }

            var messageHandler = CreateHttpMessageHandler();

            var httpClient =
                Options.ClientFactory.CreateHttpClient(messageHandler) ??
                    new HttpClient(messageHandler, false);

            // Apply this setting afterwards, to override the value from the client factory.
            httpClient.BaseAddress = url;

            // Also override timeout when create from factory.
            httpClient.Timeout = Options.HttpClientTimeout;

            Options.Configurator.Configure(httpClient);

            return httpClient;
        }

        private HttpMessageHandler CreateHttpMessageHandler()
        {
            var handler = new HttpClientHandler();

            Options.Configurator.Configure(handler);

            HttpMessageHandler messageHandler = new AuthenticatingHttpMessageHandler(Options.Authenticator)
            {
                InnerHandler = handler
            };

            return Options.ClientFactory.CreateHttpMessageHandler(messageHandler) ?? messageHandler;
        }
    }
}
