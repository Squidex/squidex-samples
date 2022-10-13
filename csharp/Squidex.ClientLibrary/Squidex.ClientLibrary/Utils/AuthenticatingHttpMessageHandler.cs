﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net;
using System.Net.Http.Headers;

namespace Squidex.ClientLibrary.Utils
{
    /// <summary>
    /// A custom message handler to handle authentication with Squidex.
    /// </summary>
    public sealed class AuthenticatingHttpMessageHandler : DelegatingHandler
    {
        private readonly IAuthenticator authenticator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatingHttpMessageHandler"/> class with the authenticator.
        /// </summary>
        /// <param name="authenticator">The authenticator. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="authenticator"/> is null.</exception>
        public AuthenticatingHttpMessageHandler(IAuthenticator authenticator)
        {
            Guard.NotNull(authenticator, nameof(authenticator));

            this.authenticator = authenticator;
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!authenticator.ShouldIntercept(request))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var appName = string.Empty;

            if (request.Headers.TryGetValues(SpecialHeaders.AppName, out var appValues))
            {
                request.Headers.Remove(SpecialHeaders.AppName);

                appName = appValues.FirstOrDefault() ?? string.Empty;
            }

            var token = await authenticator.GetBearerTokenAsync(appName, cancellationToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await authenticator.RemoveTokenAsync(appName, token, cancellationToken);
            }

            return response;
        }
    }
}
