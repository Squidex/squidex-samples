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
            return new AppsClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IAssetsClient CreateAssetsClient()
        {
            return new AssetsClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IBackupsClient CreateBackupsClient()
        {
            return new BackupsClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ICommentsClient CreateCommentsClient()
        {
            return new CommentsClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IDiagnosticsClient CreateDiagnosticsClient()
        {
            return new DiagnosticsClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IEventConsumersClient CreateEventConsumersClient()
        {
            return new EventConsumersClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IHistoryClient CreateHistoryClient()
        {
            return new HistoryClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ILanguagesClient CreateLanguagesClient()
        {
            return new LanguagesClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IPingClient CreatePingClient()
        {
            return new PingClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IPlansClient CreatePlansClient()
        {
            return new PlansClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IRulesClient CreateRulesClient()
        {
            return new RulesClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ISchemasClient CreateSchemasClient()
        {
            return new SchemasClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ISearchClient CreateSearchClient()
        {
            return new SearchClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IStatisticsClient CreateStatisticsClient()
        {
            return new StatisticsClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ITeamsClient CreateTeamsClient()
        {
            return new TeamsClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ITemplatesClient CreateTemplatesClient()
        {
            return new TemplatesClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public ITranslationsClient CreateTranslationsClient()
        {
            return new TranslationsClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IUsersClient CreateUsersClient()
        {
            return new UsersClient(Options.ClientProvider)
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        /// <inheritdoc/>
        public IUserManagementClient CreateUserManagementClient()
        {
            return new UserManagementClient(Options.ClientProvider)
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
            return new ExtendableRulesClient(Options, appName, Options.ClientProvider);
        }

        /// <inheritdoc/>
        public IContentsClient<TEntity, TData> CreateContentsClient<TEntity, TData>(string schemaName) where TEntity : Content<TData> where TData : class, new()
        {
            return CreateContentsClient<TEntity, TData>(Options.AppName, schemaName);
        }

        /// <inheritdoc/>
        public IContentsClient<TEntity, TData> CreateContentsClient<TEntity, TData>(string appName, string schemaName) where TEntity : Content<TData> where TData : class, new()
        {
            return new ContentsClient<TEntity, TData>(Options, appName, schemaName, Options.ClientProvider);
        }

        /// <inheritdoc/>
        public IContentsClient<DynamicContent, DynamicData> CreateDynamicContentsClient(string schemaName)
        {
            return CreateDynamicContentsClient(Options.AppName, schemaName);
        }

        /// <inheritdoc/>
        public IContentsClient<DynamicContent, DynamicData> CreateDynamicContentsClient(string appName, string schemaName)
        {
            return new ContentsClient<DynamicContent, DynamicData>(Options, appName, schemaName, Options.ClientProvider);
        }

        /// <inheritdoc/>
        public HttpClient CreateHttpClient()
        {
            return Options.ClientProvider.Get();
        }

        /// <inheritdoc/>
        public void ReturnHttpClient(HttpClient httpClient)
        {
            Options.ClientProvider.Return(httpClient);
        }
    }
}
