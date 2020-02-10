// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Squidex.ClientLibrary.Configuration;

namespace Squidex.ClientLibrary
{
    public sealed partial class SquidexClientManager
    {
        [Obsolete("Use Constructor with optiosn instead.")]
        public SquidexClientManager(string serviceUrl, string applicationName, string clientId, string clientSecret, IHttpConfigurator httpConfigurator = null)
            : this(new SquidexOptions { Url = serviceUrl, ClientId = clientId, ClientSecret = clientSecret, AppName = applicationName, Configurator = httpConfigurator })
        {
        }

        [Obsolete("Use Constructor with optiosn instead.")]
        public SquidexClientManager(string serviceUrl, string applicationName, IAuthenticator authenticator, IHttpConfigurator httpConfigurator = null)
            : this(new SquidexOptions { Url = serviceUrl, Authenticator = authenticator, AppName = applicationName, Configurator = httpConfigurator })
        {
        }

        [Obsolete("Use Constructor with optiosn instead.")]
        public SquidexClientManager(Uri serviceUrl, string applicationName, IAuthenticator authenticator, IHttpConfigurator httpConfigurator = null)
            : this(new SquidexOptions { Url = serviceUrl.ToString(), Authenticator = authenticator, AppName = applicationName, Configurator = httpConfigurator })
        {
        }

        [Obsolete("Use Constructor with optiosn instead.")]
        public static SquidexClientManager FromOption(SquidexOptions options)
        {
            Guard.NotNull(options, nameof(options));

            return new SquidexClientManager(options);
        }

        [Obsolete("Use CreateAssetsClient instead")]
        public SquidexAssetClient GetAssetClient()
        {
            return new SquidexAssetClient(Options, CreateHttpClient());
        }

        [Obsolete("Use CreateContentsClient instead")]
        public SquidexClient<TEntity, TData> GetClient<TEntity, TData>(string schemaName)
            where TEntity : SquidexEntityBase<TData> where TData : class, new()
        {
            Guard.NotNullOrEmpty(schemaName, nameof(schemaName));

            return new SquidexClient<TEntity, TData>(Options, schemaName, CreateHttpClient());
        }
    }
}
