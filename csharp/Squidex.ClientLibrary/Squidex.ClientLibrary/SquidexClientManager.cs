// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Squidex.ClientLibrary.Management;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary
{
    public sealed partial class SquidexClientManager
    {
        public string App
        {
            get { return Options.AppName; }
        }

        public SquidexOptions Options { get; }

        public SquidexClientManager(SquidexOptions options)
        {
            Guard.NotNull(options, nameof(options));

            options.CheckAndFreeze();

            Options = options;
        }

        public string GenerateImageUrl(string id)
        {
            if (id == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(Options.AssetCDN))
            {
                return $"{Options.AssetCDN}/{id}";
            }

            return $"{Options.Url}/api/assets/{id}";
        }

        public string GenerateImageUrl(IEnumerable<string> id)
        {
            return GenerateImageUrl(id?.FirstOrDefault());
        }

        public IAppsClient CreateAppsClient()
        {
            return new AppsClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public IAssetsClient CreateAssetsClient()
        {
            return new AssetsClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public IBackupsClient CreateBackupsClient()
        {
            return new BackupsClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public ICommentsClient CreateCommentsClient()
        {
            return new CommentsClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public IHistoryClient CreateHistoryClient()
        {
            return new HistoryClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public ILanguagesClient CreateLanguagesClient()
        {
            return new LanguagesClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public IPingClient CreatePingClient()
        {
            return new PingClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public IPlansClient CreatePlansClient()
        {
            return new PlansClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public IRulesClient CreateRulesClient()
        {
            return new RulesClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public ISchemasClient CreateSchemasClient()
        {
            return new SchemasClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public IStatisticsClient CreateStatisticsClient()
        {
            return new StatisticsClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public IUsersClient CreateUsersClient()
        {
            return new UsersClient(CreateHttpClient())
            {
                ReadResponseAsString = Options.ReadResponseAsString
            };
        }

        public IExtendableRulesClient CreateExtendableRulesClient()
        {
            return new ExtendableRulesClient(Options, CreateHttpClient());
        }

        public IContentsClient<TEntity, TData> CreateContentsClient<TEntity, TData>(string schemaName) where TEntity : Content<TData> where TData : class, new()
        {
            return new ContentsClient<TEntity, TData>(Options, schemaName, CreateHttpClient());
        }

        public HttpClient CreateHttpClient()
        {
            var url = new Uri(new Uri(Options.Url, UriKind.Absolute), "/api/");

            var handler = new AuthenticatingHttpClientHandler(Options.Authenticator);

            Options.Configurator.Configure(handler);

            var httpClient = new HttpClient(handler, false)
            {
                BaseAddress = url
            };

            Options.Configurator.Configure(httpClient);

            return httpClient;
        }
    }
}
