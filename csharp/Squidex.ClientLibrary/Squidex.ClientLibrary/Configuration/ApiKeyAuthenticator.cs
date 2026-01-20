// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.Configuration;

internal sealed class ApiKeyAuthenticator(string appName, string apiKey) : IAuthenticator
{
    private readonly Task<AuthToken> token =
        Task.FromResult<AuthToken>(new ApiKeyAuthToken(appName, apiKey));

    public Task<AuthToken> GetAuthTokenAsync(string appName, CancellationToken ct)
    {
        return token;
    }

    public Task RemoveTokenAsync(string appName, AuthToken token, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public bool ShouldIntercept(HttpRequestMessage request)
    {
        return true;
    }
}
