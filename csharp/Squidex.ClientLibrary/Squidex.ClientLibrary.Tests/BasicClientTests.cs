﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class BasicClientTests
    {
        private readonly ISquidexClientManager sut;

        public BasicClientTests()
        {
            sut = new SquidexClientManager(new SquidexOptions
            {
                AppName = "squidex-website",
                ClientId = "squidex-website:reader",
                ClientSecret = "yy9x4dcxsnp1s34r2z19t88wedbzxn1tfq7uzmoxf60x"
            });
        }

        [Fact]
        public async Task Should_make_ping_request()
        {
            var pingClient = sut.CreatePingClient();

            await pingClient.GetPingAsync();
        }

        [Fact(Skip = "No permissions.")]
        public async Task Should_make_ping_api_request()
        {
            var pingClient = sut.CreatePingClient();

            await pingClient.GetAppPingAsync("squidex-website");
        }

        [Fact]
        public async Task Should_get_content()
        {
            var result = await sut.CreateDynamicContentsClient("blog").GetAsync();

            Assert.NotEmpty(result.Items);
        }
    }
}
