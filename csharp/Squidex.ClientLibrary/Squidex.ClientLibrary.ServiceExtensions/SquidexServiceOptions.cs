// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.ServiceExtensions
{
    /// <summary>
    /// Advanced Squidex options.
    /// </summary>
    public sealed class SquidexServiceOptions : SquidexOptions
    {
        /// <summary>
        /// Gets or sets a value indicating, if the HTTP client should be configured with the authenticator.
        /// </summary>
        public bool ConfigureHttpClientWithAuthenticator { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating, if the HTTP client should be configured with the timeout.
        /// </summary>
        public bool ConfigureHttpClientWithTimeout { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating, if the HTTP client should be configured with the URL.
        /// </summary>
        public bool ConfigureHttpClientWithUrl { get; set; } = true;
    }
}
