// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// Application specific credentials.
    /// </summary>
    public sealed class AppCredentials : OptionsBase
    {
        private string clientId;
        private string clientSecret;

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier. This is a required option.
        /// </value>
        /// <exception cref="InvalidOperationException">Option is frozen and cannot be changed anymore.</exception>
        public string ClientId
        {
            get => clientId;
            set => Set(ref clientId, value);
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
            get => clientSecret;
            set => Set(ref clientSecret, value);
        }

        /// <summary>
        /// Validates the options.
        /// </summary>
        public void CheckAndFreeze()
        {
            if (IsFrozen)
            {
                return;
            }

#pragma warning disable MA0015 // Specify the parameter name in ArgumentException
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentException("Client id is not defined.", nameof(ClientId));
            }

            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new ArgumentException("Client secret is not defined.", nameof(ClientSecret));
            }

            Freeze();
#pragma warning restore MA0015 // Specify the parameter name in ArgumentException
        }
    }
}
