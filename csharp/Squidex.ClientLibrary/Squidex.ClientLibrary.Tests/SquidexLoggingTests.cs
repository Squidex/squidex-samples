// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.DependencyInjection;
using Squidex.ClientLibrary.Configuration;

namespace Squidex.ClientLibrary.Tests;

public class SquidexLoggingTests
{
    [Fact]
    public async Task Should_log_with_services()
    {
        var loggingHandler = new SampleLoggingHandler();

        var sut =
            new ServiceCollection()
                .AddSquidexClient(options =>
                {
                    options.AppName = "invalid";
                    options.ClientId = "invalid";
                    options.ClientSecret = "invalid";
                })
                .AddSquidexHttpClient()
                    .AddHttpMessageHandler(() => loggingHandler)
                    .Services
                .BuildServiceProvider()
                .GetRequiredService<ISquidexClient>();

        try
        {
            await sut.Ping.GetAppPingAsync();
        }
        catch
        {
            // Invalid Credentials
        }

        Assert.NotEmpty(loggingHandler.Log);
        Assert.Contains(loggingHandler.Log, x => x.Url.Contains("identity-server/connect/token", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Should_log_with_manual_client()
    {
        var loggingHandler = new SampleLoggingHandler();

        var options = new SquidexOptions
        {
            AppName = "invalid",
            ClientId = "invalid",
            ClientSecret = "invalid"
        };

        options.ClientProvider = new ClientProvider(options, loggingHandler);

        var sut = new SquidexClient(options);

        try
        {
            await sut.Ping.GetAppPingAsync();
        }
        catch
        {
            // Invalid Credentials
        }

        Assert.NotEmpty(loggingHandler.Log);
        Assert.Contains(loggingHandler.Log, x => x.Url.Contains("identity-server/connect/token", StringComparison.Ordinal));
    }

    private class ClientProvider : StaticHttpClientProvider
    {
        private readonly SampleLoggingHandler sampleLoggingHandler;

        public ClientProvider(SquidexOptions options, SampleLoggingHandler sampleLoggingHandler)
            : base(options)
        {
            this.sampleLoggingHandler = sampleLoggingHandler;
        }

        protected override HttpMessageHandler CreateMessageHandler(SquidexOptions options)
        {
            sampleLoggingHandler.InnerHandler = base.CreateMessageHandler(options);

            return sampleLoggingHandler;
        }
    }
}
