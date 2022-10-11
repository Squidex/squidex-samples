// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Configuration;
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
        private readonly IHttpClientProvider httpClientProvider;

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

            httpClientProvider = options.ClientProvider ?? new StaticHttpClientProvider(options);
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

        private static string? GenerateUrl(string baseUrl, string? relativeUrl)
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
            return new AppsClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IAssetsClient CreateAssetsClient()
        {
            return new AssetsClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IBackupsClient CreateBackupsClient()
        {
            return new BackupsClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ICommentsClient CreateCommentsClient()
        {
            return new CommentsClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IDiagnosticsClient CreateDiagnosticsClient()
        {
            return new DiagnosticsClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IEventConsumersClient CreateEventConsumersClient()
        {
            return new EventConsumersClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IHistoryClient CreateHistoryClient()
        {
            return new HistoryClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ILanguagesClient CreateLanguagesClient()
        {
            return new LanguagesClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IPingClient CreatePingClient()
        {
            return new PingClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IPlansClient CreatePlansClient()
        {
            return new PlansClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IRulesClient CreateRulesClient()
        {
            return new RulesClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ISchemasClient CreateSchemasClient()
        {
            return new SchemasClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ISearchClient CreateSearchClient()
        {
            return new SearchClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IStatisticsClient CreateStatisticsClient()
        {
            return new StatisticsClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ITeamsClient CreateTeamsClient()
        {
            return new TeamsClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ITemplatesClient CreateTemplatesClient()
        {
            return new TemplatesClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ITranslationsClient CreateTranslationsClient()
        {
            return new TranslationsClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IUsersClient CreateUsersClient()
        {
            return new UsersClient(httpClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IUserManagementClient CreateUserManagementClient()
        {
            return new UserManagementClient(httpClientProvider)
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
            return new ExtendableRulesClient(Options, appName, httpClientProvider);
        }

        /// <inheritdoc/>
        public IContentsClient<TEntity, TData> CreateContentsClient<TEntity, TData>(string schemaName) where TEntity : Content<TData> where TData : class, new()
        {
            return CreateContentsClient<TEntity, TData>(Options.AppName, schemaName);
        }

        /// <inheritdoc/>
        public IContentsClient<TEntity, TData> CreateContentsClient<TEntity, TData>(string appName, string schemaName) where TEntity : Content<TData> where TData : class, new()
        {
            return new ContentsClient<TEntity, TData>(Options, appName, schemaName, httpClientProvider);
        }

        /// <inheritdoc/>
        public IContentsClient<DynamicContent, DynamicData> CreateDynamicContentsClient(string schemaName)
        {
            return CreateDynamicContentsClient(Options.AppName, schemaName);
        }

        /// <inheritdoc/>
        public IContentsClient<DynamicContent, DynamicData> CreateDynamicContentsClient(string appName, string schemaName)
        {
            return new ContentsClient<DynamicContent, DynamicData>(Options, appName, schemaName, httpClientProvider);
        }

        /// <inheritdoc/>
        public HttpClient CreateHttpClient()
        {
            return httpClientProvider.Get();
        }

        /// <inheritdoc/>
        public void ReturnHttpClient(HttpClient httpClient)
        {
            httpClientProvider.Return(httpClient);
        }
    }
}
