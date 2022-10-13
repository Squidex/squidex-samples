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
    public class ContentQueryIntegrationTests
    {
        [Fact]
        public async Task Should_query_content()
        {
            var sut = CreateSut().CreateDynamicContentsClient("blog");

            var result = await sut.GetAsync();

            Assert.NotEmpty(result.Items);
        }

        private ISquidexClientManager CreateSut()
        {
            var sut =
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

            return sut;
        }
    }
}
