// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Squidex.ClientLibrary.Configuration;

namespace Squidex.ClientLibrary
{
    public sealed class SquidexOptions
    {
        private string url = "https://cloud.squidex.io ";
        private string appName;
        private string clientId;
        private string clientSecret;
        private string contentCDN;
        private string assetCDN;
        private bool readResponseAsString;
        private IAuthenticator authenticator;
        private IHttpConfigurator configurator;
        private IHttpClientFactory clientFactory;
        private TimeSpan httpClientTimeOut;
        private bool isFrozen;

        public string Url { get => url; set => url = value; }

        public string AppName
        {
            get
            {
                return appName;
            }

            set
            {
                ThrowIfFrozen();

                appName = value;
            }
        }

        public string ClientId
        {
            get
            {
                return clientId;
            }
            set
            {
                ThrowIfFrozen();

                clientId = value;
            }
        }

        public string ClientSecret
        {
            get
            {
                return clientSecret;
            }
            set
            {
                ThrowIfFrozen();

                clientSecret = value;
            }
        }

        public string ContentCDN
        {
            get
            {
                return contentCDN;
            }
            set
            {
                ThrowIfFrozen();

                contentCDN = value;
            }
        }

        public string AssetCDN
        {
            get
            {
                return assetCDN;
            }
            set
            {
                ThrowIfFrozen();

                assetCDN = value;
            }
        }

        public bool ReadResponseAsString
        {
            get
            {
                return readResponseAsString;
            }
            set
            {
                ThrowIfFrozen();

                readResponseAsString = value;
            }
        }

        public IAuthenticator Authenticator
        {
            get
            {
                return authenticator;
            }
            set
            {
                ThrowIfFrozen();

                authenticator = value;
            }
        }

        public IHttpConfigurator Configurator
        {
            get
            {
                return configurator;
            }
            set
            {
                ThrowIfFrozen();

                configurator = value;
            }
        }

        public IHttpClientFactory ClientFactory
        {
            get
            {
                return clientFactory;
            }
            set
            {
                ThrowIfFrozen();

                clientFactory = value;
            }
        }

        public TimeSpan HttpClientTimeOut
        {
            get => httpClientTimeOut;
            set
            {
                ThrowIfFrozen();

                httpClientTimeOut = value;
            }
        }

        private void ThrowIfFrozen()
        {
            if (isFrozen)
            {
                throw new InvalidOperationException("Options are frozen and cannot be changed.");
            }
        }

        public void CheckAndFreeze()
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new ArgumentException("URL must be a valid absolute URL.");
            }
            else
            {
                url = url.TrimEnd('/', ' ');
            }

            if (string.IsNullOrWhiteSpace(appName))
            {
                throw new ArgumentException("App name is not defined.");
            }

            if (!string.IsNullOrWhiteSpace(assetCDN))
            {
                if (!Uri.IsWellFormedUriString(assetCDN, UriKind.Absolute))
                {
                    throw new ArgumentException("Asset CDN URL must be absolute if specified.");
                }

                contentCDN = contentCDN.TrimEnd('/', ' ');
            }

            if (!string.IsNullOrWhiteSpace(contentCDN))
            {
                if (!Uri.IsWellFormedUriString(contentCDN, UriKind.Absolute))
                {
                    throw new ArgumentException("Content CDN URL must be absolute if specified.");
                }

                assetCDN = assetCDN.TrimEnd('/', ' ');
            }

            if (authenticator == null)
            {
                if (string.IsNullOrWhiteSpace(clientId))
                {
                    throw new ArgumentException("Client id is not defined.");
                }

                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    throw new ArgumentException("Client secret is not defined.");
                }

                var cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

                authenticator = new CachingAuthenticator($"TOKEN_{Url}", cache, new Authenticator(Url, clientId, clientSecret));
            }

            if (configurator == null)
            {
                configurator = NoopHttpConfigurator.Instance;
            }

            if (clientFactory == null)
            {
                clientFactory = NoopHttpConfigurator.Instance;
            }

            if (httpClientTimeOut == TimeSpan.Zero)
            {
                httpClientTimeOut = TimeSpan.FromSeconds(100);
            }

            isFrozen = true;
        }
    }
}
