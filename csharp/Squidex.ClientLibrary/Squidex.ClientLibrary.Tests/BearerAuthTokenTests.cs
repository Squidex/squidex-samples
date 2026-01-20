// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.Tests;

public class BearerAuthTokenTests
{
    [Fact]
    public void Should_serialize_as_header()
    {
        var sut = new BearerAuthToken("MyToken");

        var header = sut.SerializeAsHeader();
        Assert.Equal(("Authorization", "Bearer MyToken"), header);
    }

    [Fact]
    public void Should_serialize_as_query()
    {
        var sut = new BearerAuthToken("MyToken");

        var header = sut.SerializeAsQuery();
        Assert.Equal(("access_token", "MyToken"), header);
    }
}
