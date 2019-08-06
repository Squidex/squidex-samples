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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Squidex.ClientLibrary.Management;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary
{
    public sealed class SquidexClientManager
    {
        private readonly string applicationName;
        private readonly Uri serviceUrl;
        private readonly IAuthenticator authenticator;
        private readonly bool acceptAllCertificates;

        public bool ReadResponseAsString { get; set; }

        public string App
        {
            get { return applicationName; }
        }

        public SquidexClientManager(string serviceUrl, string applicationName, string clientId, string clientSecret, bool acceptAllCertificates = false)
            : this(new Uri(serviceUrl, UriKind.Absolute), applicationName,
                  new CachingAuthenticator($"TOKEN_{serviceUrl}", new MemoryCache(Options.Create(new MemoryCacheOptions())),
                      new Authenticator(serviceUrl, clientId, clientSecret)),
                  acceptAllCertificates)
        {
        }

        public SquidexClientManager(string serviceUrl, string applicationName, IAuthenticator authenticator, bool acceptAllCertificates = false)
            : this(new Uri(serviceUrl, UriKind.Absolute), applicationName, authenticator, acceptAllCertificates)
        {
        }

        public SquidexClientManager(Uri serviceUrl, string applicationName, IAuthenticator authenticator, bool acceptAllCertificates = false)
        {
            Guard.NotNull(serviceUrl, nameof(serviceUrl));
            Guard.NotNull(authenticator, nameof(authenticator));
            Guard.NotNullOrEmpty(applicationName, nameof(applicationName));

            this.authenticator = authenticator;
            this.applicationName = applicationName;
            this.serviceUrl = serviceUrl;
            this.acceptAllCertificates = acceptAllCertificates;
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
            return new AppsClient(CreateHttpClient()) { ReadResponseAsString = ReadResponseAsString };
        }

        public IBackupsClient CreateBackupsClient()
        {
            return new BackupsClient(CreateHttpClient()) { ReadResponseAsString = ReadResponseAsString };
        }

        public ICommentsClient CreateCommentsClient()
        {
            return new CommentsClient(CreateHttpClient()) { ReadResponseAsString = ReadResponseAsString };
        }

        public IHistoryClient CreateHistoryClient()
        {
            return new HistoryClient(CreateHttpClient()) { ReadResponseAsString = ReadResponseAsString };
        }

        public ILanguagesClient CreateLanguagesClient()
        {
            return new LanguagesClient(CreateHttpClient()) { ReadResponseAsString = ReadResponseAsString };
        }

        public IPingClient CreatePingClient()
        {
            return new PingClient(CreateHttpClient()) { ReadResponseAsString = ReadResponseAsString };
        }

        public IPlansClient CreatePlansClient()
        {
            return new PlansClient(CreateHttpClient()) { ReadResponseAsString = ReadResponseAsString };
        }

        public IRulesClient CreateRulesClient()
        {
            return new RulesClient(CreateHttpClient()) { ReadResponseAsString = ReadResponseAsString };
        }

        public ISchemasClient CreateSchemasClient()
        {
            return new SchemasClient(CreateHttpClient()) { ReadResponseAsString = ReadResponseAsString };
        }

        public IStatisticsClient CreateStatisticsClient()
        {
            return new StatisticsClient(CreateHttpClient()) { ReadResponseAsString = ReadResponseAsString };
        }

        public IUsersClient CreateUsersClient()
        {
            return new UsersClient(CreateHttpClient()) { ReadResponseAsString = ReadResponseAsString };
        }

        public SquidexAssetClient GetAssetClient()
        {
            return new SquidexAssetClient(applicationName, CreateHttpClient());
        }

        public SquidexClient<TEntity, TData> GetClient<TEntity, TData>(string schemaName)
            where TEntity : SquidexEntityBase<TData>
            where TData : class, new()
        {
            Guard.NotNullOrEmpty(schemaName, nameof(schemaName));

            return new SquidexClient<TEntity, TData>(applicationName, schemaName, CreateHttpClient());
        }

        public HttpClient CreateHttpClient()
        {
            var url = new Uri(serviceUrl, "/api/");

            return new HttpClient(new AuthenticatingHttpClientHandler(authenticator, acceptAllCertificates), false)
            {
                BaseAddress = url
            };
        }
    }
}
