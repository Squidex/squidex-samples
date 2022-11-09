// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.Configuration;
using Xunit;

namespace Squidex.ClientLibrary.Tests;

public class SquidexOptionsTests
{
    [Fact]
    public void Should_bind_to_config()
    {
        var configuration =
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("TestFile.json")
                .Build();

        var options = new SquidexOptions();

        configuration.GetSection("squidex").Bind(options);

        options.CheckAndFreeze();

        Assert.Equal("My-App", options.AppName);
        Assert.Equal("My-ClientId", options.ClientId);
        Assert.Equal("My-ClientSecret", options.ClientSecret);

        var appCredentials = options.AppCredentials!["My-App2"];

        Assert.Equal("My-ClientId2", appCredentials.ClientId);
        Assert.Equal("My-ClientSecret2", appCredentials.ClientSecret);
    }
}
