// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net;

namespace Squidex.ClientLibrary.Utils;

/// <summary>
/// A custom message handler to handle authentication with Squidex.
/// </summary>
public sealed class AuthenticatingHttpMessageHandler : DelegatingHandler
{
    private readonly SquidexOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticatingHttpMessageHandler"/> class with the authenticator.
    /// </summary>
    /// <param name="options">The options. Cannot be null.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public AuthenticatingHttpMessageHandler(SquidexOptions options)
    {
        Guard.NotNull(options, nameof(options));

        this.options = options;
    }

    /// <inheritdoc/>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization != null)
        {
            return base.SendAsync(request, cancellationToken);
        }

        if (!options.Authenticator.ShouldIntercept(request))
        {
            return base.SendAsync(request, cancellationToken);
        }

        return InterceptAsync(request, true, cancellationToken);
    }

    private async Task<HttpResponseMessage> InterceptAsync(HttpRequestMessage request, bool retry,
        CancellationToken cancellationToken)
    {
        var token = await options.Authenticator.GetAuthTokenAsync(options.AppName, cancellationToken);

        var (name, value) = token.SerializeAsHeader();
        request.Headers.TryAddWithoutValidation(name, value);

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await options.Authenticator.RemoveTokenAsync(options.AppName, token, cancellationToken);

            if (retry)
            {
                return await InterceptAsync(request, false, cancellationToken);
            }
        }

        return response;
    }
}
