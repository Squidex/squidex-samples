// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class BasicClientServiceTests
    {
        private readonly ISquidexClientManager sut;

        public BasicClientServiceTests()
        {
            sut =
               new ServiceCollection()
                   .AddSquidexClient(options =>
                   {
                       options.AppName = "squidex-website";
                       options.ClientId = "squidex-website:default";
                       options.ClientSecret = "QGgqxd7bDHBTEkpC6fj8sbdPWgZrPrPfr3xzb3LKoec=";
                       options.Url = "https://cloud.squidex.io";
                   })
                   .BuildServiceProvider()
                   .GetRequiredService<ISquidexClientManager>();
        }

        [Fact]
        public async Task Should_query_content()
        {
            var result = await sut.CreateDynamicContentsClient("blog").GetAsync();

            Assert.NotEmpty(result.Items);
        }
    }
}
