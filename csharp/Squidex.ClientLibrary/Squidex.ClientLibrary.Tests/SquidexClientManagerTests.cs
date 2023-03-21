﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.DependencyInjection;
using Squidex.ClientLibrary.ServiceExtensions;
using Xunit;

namespace Squidex.ClientLibrary.Tests;

public class SquidexClientManagerTests
{
    [Fact]
    public void Should_build_client_from_service()
    {
        var sut =
            new ServiceCollection()
                .AddSquidexClient(options =>
                {
                    options.AppName = "app";
                    options.ClientId = "id";
                    options.ClientSecret = "secret";
                    options.Url = "https://custom.squidex.io";
                })
                .BuildServiceProvider()
                .GetRequiredService<ISquidexClient>();

        Assert.Equal("https://custom.squidex.io/", sut.Options.Url);
    }

    [Fact]
    public void Should_build_client_from_service_with_custom_options()
    {
        var sut =
            new ServiceCollection()
                .AddSquidexClient(null)
                .Configure<SquidexServiceOptions>(options =>
                {
                    options.AppName = "app";
                    options.ClientId = "id";
                    options.ClientSecret = "secret";
                    options.Url = "https://custom.squidex.io";
                })
                .BuildServiceProvider()
                .GetRequiredService<ISquidexClient>();

        Assert.Equal("https://custom.squidex.io/", sut.Options.Url);
    }

    [Fact]
    public void Should_build_client_from_named_service()
    {
        var provider =
            new ServiceCollection()
                .AddSquidexClient(null)
                .Configure<SquidexServiceOptions>(options =>
                {
                    options.AppName = "app";
                    options.ClientId = "id";
                    options.ClientSecret = "secret";
                    options.Url = "https://custom1.squidex.io";
                })
                .Configure<SquidexServiceOptions>("named", options =>
                {
                    options.AppName = "app";
                    options.ClientId = "id";
                    options.ClientSecret = "secret";
                    options.Url = "https://custom2.squidex.io";
                })
                .BuildServiceProvider()
                .GetRequiredService<ISquidexClientProvider>();

        var sut1 = provider.Get();
        var sut2 = provider.Get("named");

        Assert.Equal("https://custom1.squidex.io/", sut1.Options.Url);
        Assert.Equal("https://custom2.squidex.io/", sut2.Options.Url);
    }

    [Theory]
    [InlineData("https://cloud.squidex.io")]
    [InlineData("https://cloud.squidex.io/")]
    public void Should_generate_url_without_leading_edge(string url)
    {
        var sut = BuildClientManager(options => options.Url = url);

        var result = sut.GenerateUrl("relative/url");

        Assert.Equal("https://cloud.squidex.io/relative/url", result);
    }

    [Theory]
    [InlineData("https://cloud.squidex.io")]
    [InlineData("https://cloud.squidex.io/")]
    public void Should_generate_url_with_leading_edge(string url)
    {
        var sut = BuildClientManager(options => options.Url = url);

        var result = sut.GenerateUrl("/relative/url");

        Assert.Equal("https://cloud.squidex.io/relative/url", result);
    }

    [Theory]
    [InlineData("https://contents.cdn.squidex.io")]
    [InlineData("https://contents.cdn.squidex.io/")]
    public void Should_generate_content_cdn_url_without_leading_edge(string url)
    {
        var sut = BuildClientManager(options => options.ContentCDN = url);

        var result = sut.GenerateContentCDNUrl("relative/url");

        Assert.Equal("https://contents.cdn.squidex.io/relative/url", result);
    }

    [Theory]
    [InlineData("https://contents.cdn.squidex.io")]
    [InlineData("https://contents.cdn.squidex.io/")]
    public void Should_generate_content_cdn_url_with_leading_edge(string url)
    {
        var sut = BuildClientManager(options => options.ContentCDN = url);

        var result = sut.GenerateContentCDNUrl("/relative/url");

        Assert.Equal("https://contents.cdn.squidex.io/relative/url", result);
    }

    [Fact]
    public void Should_throw_exception_if_content_cdn_not_configured()
    {
        var sut = BuildClientManager();

        Assert.Throws<InvalidOperationException>(() => sut.GenerateContentCDNUrl("relative/url"));
    }

    [Theory]
    [InlineData("https://assets.cdn.squidex.io")]
    [InlineData("https://assets.cdn.squidex.io/")]
    public void Should_generate_asset_cdn_url_without_leading_edge(string url)
    {
        var sut = BuildClientManager(options => options.AssetCDN = url);

        var result = sut.GenerateAssetCDNUrl("relative/url");

        Assert.Equal("https://assets.cdn.squidex.io/relative/url", result);
    }

    [Theory]
    [InlineData("https://assets.cdn.squidex.io")]
    [InlineData("https://assets.cdn.squidex.io/")]
    public void Should_generate_asset_cdn_url_with_leading_edge(string url)
    {
        var sut = BuildClientManager(options => options.AssetCDN = url);

        var result = sut.GenerateAssetCDNUrl("/relative/url");

        Assert.Equal("https://assets.cdn.squidex.io/relative/url", result);
    }

    [Fact]
    public void Should_throw_exception_if_asset_cdn_not_configured()
    {
        var sut = BuildClientManager();

        Assert.Throws<InvalidOperationException>(() => sut.GenerateAssetCDNUrl("relative/url"));
    }

    private static SquidexClient BuildClientManager(Action<SquidexOptions>? configure = null)
    {
        var options = new SquidexOptions
        {
            AppName = "app",
            ClientId = "id",
            ClientSecret = "secret",
            Url = "https://squidex.io"
        };

        configure?.Invoke(options);

        var sut = new SquidexClient(options);

        return sut;
    }
}
