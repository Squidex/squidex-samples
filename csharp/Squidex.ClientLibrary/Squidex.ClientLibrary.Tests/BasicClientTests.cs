// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Configuration;

namespace Squidex.ClientLibrary.Tests;

public class BasicClientTests
{
    private readonly ISquidexClient sut;

    public BasicClientTests()
    {
        sut = new SquidexClient(new SquidexOptions
        {
            AppName = "squidex-website",
            ClientId = "squidex-website:reader",
            ClientSecret = "yy9x4dcxsnp1s34r2z19t88wedbzxn1tfq7uzmoxf60x",
        });
    }

    [Fact(Skip = "No permissions.")]
    public async Task Should_make_ping_api_request()
    {
        await sut.Ping.GetAppPingAsync();
    }

    [Fact]
    public async Task Should_make_ping_request()
    {
        await sut.Ping.GetPingAsync();
    }

    [Fact]
    public async Task Should_get_content()
    {
        var result = await sut.DynamicContents("blog").GetAsync();

        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task Should_get_content_with_invalid_token_for_anonymous_access()
    {
        ((CachingAuthenticator)sut.Options.Authenticator).SetToCache(sut.Options.AppName,
            new AuthToken("Authorization", "Bearer TOKEN"), DateTimeOffset.MaxValue);

        var result = await sut.DynamicContents("blog").GetAsync();

        Assert.NotEmpty(result.Items);
    }
}
