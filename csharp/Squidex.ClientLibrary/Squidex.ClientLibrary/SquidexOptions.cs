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
    /// <summary>
    /// The options to configure <see cref="SquidexClientManager"/>.
    /// </summary>
    public sealed class SquidexOptions
    {
        private string url = "https://cloud.squidex.io";
        private string appName;
        private string clientId;
        private string clientSecret;
        private string contentCDN;
        private string assetCDN;
        private bool readResponseAsString;
        private IAuthenticator authenticator;
        private IHttpConfigurator configurator;
        private IHttpClientFactory clientFactory;
        private TimeSpan httpClientTimeout;
        private bool isFrozen;

        /// <summary>
        /// Gets or sets the URL to the Squidex installation.
        /// </summary>
        /// <value>
        /// The URL to the Squidex installation.
        /// </value>
        /// <remarks>
        /// Defaults to 'https://cloud.squidex.io'.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
        public string Url
        {
            get
            {
                return url;
            }

            set
            {
                ThrowIfFrozen();

                url = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application. This is a required option.
        /// </value>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
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

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier. This is a required option.
        /// </value>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
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

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret. This is a required option.
        /// </value>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
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

        /// <summary>
        /// Gets or sets the optional URL to the content CDN.
        /// </summary>
        /// <value>
        /// The optional URL to the content CDN.
        /// </value>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
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

        /// <summary>
        /// Gets or sets the optional URL to the asset CDN.
        /// </summary>
        /// <value>
        /// The optional URL to the asset CDN.
        /// </value>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
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

        /// <summary>
        /// Gets or sets a value indicating whether responses are read as string.
        /// </summary>
        /// <value>
        ///   <c>true</c> if responses are read as string; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This is useful for debugging but has impacts to performance.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
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

        /// <summary>
        /// Gets or sets the authenticator.
        /// </summary>
        /// <value>
        /// The authenticator. This is a required option.
        /// </value>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
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

        /// <summary>
        /// Gets or sets the configurator that can be used to make changes to the HTTP requests.
        /// </summary>
        /// <value>
        /// The configurator.
        /// </value>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
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

        /// <summary>
        /// Gets or sets the client factory.
        /// </summary>
        /// <value>
        /// The client factory.
        /// </value>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
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

        /// <summary>
        /// Gets or sets the HTTP client timeout.
        /// </summary>
        /// <value>
        /// The HTTP client timeout.
        /// </value>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
        public TimeSpan HttpClientTimeout
        {
            get
            {
                return httpClientTimeout;
            }
            set
            {
                ThrowIfFrozen();

                httpClientTimeout = value;
            }
        }

        private void ThrowIfFrozen()
        {
            if (isFrozen)
            {
                throw new InvalidOperationException("Options are frozen and cannot be changed.");
            }
        }

        internal void CheckAndFreeze()
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new ArgumentException("URL must be a valid absolute URL.");
            }

            url = url.TrimEnd('/', ' ');

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

            if (configurator == null)
            {
                configurator = NoopHttpConfigurator.Instance;
            }

            if (clientFactory == null)
            {
                clientFactory = NoopHttpConfigurator.Instance;
            }

            if (httpClientTimeout == TimeSpan.Zero)
            {
                httpClientTimeout = TimeSpan.FromSeconds(100);
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

                var squidexAuthenticator = new Authenticator(this);

                authenticator = new CachingAuthenticator($"TOKEN_{Url}", cache, squidexAuthenticator);
            }

            isFrozen = true;
        }
    }
}
