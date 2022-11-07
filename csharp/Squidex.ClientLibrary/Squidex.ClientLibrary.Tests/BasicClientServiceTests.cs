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
        [Fact]
        public async Task Should_query_content()
        {
            var sut = CreateSut();

            var result = await sut.CreateDynamicContentsClient("blog").GetAsync();

            Assert.NotEmpty(result.Items);
        }

        [Fact]
        public async Task Should_query_content_with_overriden_credentials()
        {
            var sut = CreateSutWithOverridenCredentials();

            var result = await sut.CreateDynamicContentsClient("blog").GetAsync();

            Assert.NotEmpty(result.Items);
        }

        private static ISquidexClientManager CreateSut()
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

        private static ISquidexClientManager CreateSutWithOverridenCredentials()
        {
            var sut =
               new ServiceCollection()
                   .AddSquidexClient(options =>
                   {
                       options.AppName = "squidex-website";
                       options.ClientId = "INVALID";
                       options.ClientSecret = "INVALID";
                       options.AppCredentials = new Dictionary<string, AppCredentials>
                       {
                           ["squidex-website"] = new AppCredentials
                           {
                               ClientId = "squidex-website:default",
                               ClientSecret = "QGgqxd7bDHBTEkpC6fj8sbdPWgZrPrPfr3xzb3LKoec=",
                           }
                       };
                       options.Url = "https://cloud.squidex.io";
                   })
                   .BuildServiceProvider()
                   .GetRequiredService<ISquidexClientManager>();

            return sut;
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(0, 10)]
        public async Task Should_not_try_authenticate_again_if_it_failed(int retryHours, int expectedAuthCalls)
        {
            var counts = new CountUrlHandler();

            var sut =
               new ServiceCollection()
                   .AddSquidexClient(options =>
                   {
                       options.AppName = "squidex-website";
                       options.ClientId = "INVALID";
                       options.ClientSecret = "INVALID";
                       options.TokenRetryTime = TimeSpan.FromHours(retryHours);
                       options.Url = "https://cloud.squidex.io";
                   })
                   .AddSquidexHttpClient()
                        .AddHttpMessageHandler(() =>
                        {
                            return counts;
                        }).Services
                   .BuildServiceProvider()
                   .GetRequiredService<ISquidexClientManager>();

            for (var i = 0; i < 10; i++)
            {
                try
                {
                    await sut.CreatePingClient().GetAppPingAsync("app");
                }
                catch
                {
                    continue;
                }
            }

            Assert.Equal(expectedAuthCalls, counts.Counts["/identity-server/connect/token"]);
        }

        private class CountUrlHandler : DelegatingHandler
        {
            public Dictionary<string, int> Counts { get; private set; } = new Dictionary<string, int>();

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var path = request.RequestUri?.PathAndQuery ?? string.Empty;

                Counts[path] = Counts.GetValueOrDefault(path) + 1;

                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
