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
    public sealed class SquidexClientManager
    {
        private readonly string applicationName;
        private readonly Uri serviceUrl;
        private readonly IAuthenticator authenticator;
        private readonly HttpMessageHandler messageHandler;

        public string App
        {
            get { return applicationName; }
        }

        public SquidexClientManager(string serviceUrl, string applicationName, string clientId, string clientSecret)
            : this(new Uri(serviceUrl, UriKind.Absolute), applicationName, new CachingAuthenticator(serviceUrl, clientId, clientSecret))
        {
        }

        public SquidexClientManager(string serviceUrl, string applicationName, IAuthenticator authenticator)
            : this(new Uri(serviceUrl, UriKind.Absolute), applicationName, authenticator)
        {
        }

        public SquidexClientManager(Uri serviceUrl, string applicationName, IAuthenticator authenticator)
        {
            Guard.NotNull(serviceUrl, nameof(serviceUrl));
            Guard.NotNull(authenticator, nameof(authenticator));
            Guard.NotNullOrEmpty(applicationName, nameof(applicationName));

            this.authenticator = authenticator;
            this.applicationName = applicationName;
            this.serviceUrl = serviceUrl;

            messageHandler = new AuthenticatingHttpClientHandler(authenticator);
        }

        public string GenerateImageUrl(string id)
        {
            return id != null ? $"{serviceUrl}api/assets/{id}" : id;
        }

        public string GenerateImageUrl(IEnumerable<string> id)
        {
            return GenerateImageUrl(id?.FirstOrDefault());
        }

        public static SquidexClientManager FromOption(SquidexOptions options)
        {
            Guard.NotNull(options, nameof(options));

            return new SquidexClientManager(
                options.Url,
                options.AppName,
                options.ClientId,
                options.ClientSecret);
        }

        public IAppsClient CreateAppsClient()
        {
            return new AppsClient(CreateHttpClient());
        }

        public IBackupsClient CreateAppsContributorsClient()
        {
            return new BackupsClient(CreateHttpClient());
        }

        public ICommentsClient CreateCommentsClient()
        {
            return new CommentsClient(CreateHttpClient());
        }

        public IHistoryClient CreateHistoryClient()
        {
            return new HistoryClient(CreateHttpClient());
        }

        public ILanguagesClient CreateLanguagesClient()
        {
            return new LanguagesClient(CreateHttpClient());
        }

        public IPingClient CreatePingClient()
        {
            return new PingClient(CreateHttpClient());
        }

        public IPlansClient CreatePlansClient()
        {
            return new PlansClient(CreateHttpClient());
        }

        public IRulesClient CreateRulesClient()
        {
            return new RulesClient(CreateHttpClient());
        }

        public ISchemasClient CreateSchemasClient()
        {
            return new SchemasClient(CreateHttpClient());
        }

        public IStatisticsClient CreateStatisticsClient()
        {
            return new StatisticsClient(CreateHttpClient());
        }

        public IUsersClient CreateUsersClient()
        {
            return new UsersClient(CreateHttpClient());
        }

        public SquidexAssetClient GetAssetClient()
        {
            return new SquidexAssetClient(serviceUrl, applicationName, string.Empty, messageHandler);
        }

        public SquidexClient<TEntity, TData> GetClient<TEntity, TData>(string schemaName)
            where TEntity : SquidexEntityBase<TData>
            where TData : class, new()
        {
            Guard.NotNullOrEmpty(schemaName, nameof(schemaName));

            return new SquidexClient<TEntity, TData>(serviceUrl, applicationName, schemaName, messageHandler);
        }

        private HttpClient CreateHttpClient()
        {
            var url = new Uri(serviceUrl, "/api/");

            return new HttpClient(messageHandler) { BaseAddress = url };
        }
    }
}
