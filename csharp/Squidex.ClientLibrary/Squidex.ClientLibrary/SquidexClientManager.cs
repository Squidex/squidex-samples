// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary
{
    public sealed class SquidexClientManager
    {
        private readonly string applicationName;
        private readonly Uri serviceUrl;
        private readonly IAuthenticator authenticator;

        public SquidexClientManager(string serviceUrl, string applicationName, string clientId, string clientSecret)
            : this(new Uri(serviceUrl, UriKind.Absolute), applicationName, new Authenticator(serviceUrl, clientId, clientSecret))
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
        }

        public SquidexClient<TEntity, TData> GetClient<TEntity, TData>(string schemaName) where TData : class, new() where TEntity : SquidexEntityBase<TData>
        {
            return new SquidexClient<TEntity, TData>(serviceUrl, applicationName, schemaName, authenticator);
        }
    }
}
