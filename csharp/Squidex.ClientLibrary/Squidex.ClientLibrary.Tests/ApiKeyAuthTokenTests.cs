// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.Tests;

public class ApiKeyAuthTokenTests
{
    [Fact]
    public void Should_serialize_as_header()
    {
        var sut = new ApiKeyAuthToken("MyApp", "MyKey");

        var header = sut.SerializeAsHeader();
        Assert.Equal(("Authorization", "ApiKey MyApp:MyKey"), header);
    }

    [Fact]
    public void Should_serialize_as_query()
    {
        var sut = new ApiKeyAuthToken("MyApp", "MyKey");

        var header = sut.SerializeAsQuery();
        Assert.Equal(("api_key", "MyApp%3AMyKey"), header);
    }
}
