// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.Tests;

public class QueryTests
{
    [Fact]
    public void Should_build_empty_query()
    {
        var q = Query.Create().ToString();

        Assert.Equal(string.Empty, q);
    }

    [Fact]
    public void Should_build_query1()
    {
        var q = Query.Create().Append("k1", "v1").ToString();

        Assert.Equal("?k1=v1", q);
    }

    [Fact]
    public void Should_build_query2()
    {
        var q = Query.Create().Append("k1", "v1").Append("k2", "v2").ToString();

        Assert.Equal("?k1=v1&k2=v2", q);
    }

    [Fact]
    public void Should_build_escaped()
    {
        var q = Query.Create().Append("k1", "v1").Append("k2", "v2", true).ToString();

        Assert.Equal("?k1=v1&k2=\"v2\"", q);
    }

    [Fact]
    public void Should_not_append_default_boolean()
    {
        var q = Query.Create().Append("k1", false).Append("k2", true).ToString();

        Assert.Equal("?k2=true", q);
    }

    [Fact]
    public void Should_not_append_default_number()
    {
        var q = Query.Create().Append("k1", 0).Append("k2", 42).ToString();

        Assert.Equal("?k2=42", q);
    }

    [Fact]
    public void Should_not_append_default_nullable()
    {
        var q = Query.Create().Append("k1", default(int?)).Append("k2", (int?)42).ToString();

        Assert.Equal("?k2=42", q);
    }

    [Fact]
    public void Should_build_list()
    {
        var q = Query.Create().AppendMany("k1", new List<int> { 1, 2, 3 }).ToString();

        Assert.Equal("?k1=1%2C2%2C3", q);
    }

    [Fact]
    public void Should_not_build_null_list()
    {
        var q = Query.Create().AppendMany("k1", (List<int>)null!).ToString();

        Assert.Equal(string.Empty, q);
    }

    [Fact]
    public void Should_not_build_empty_list()
    {
        var q = Query.Create().AppendMany("k1", new List<int>()).ToString();

        Assert.Equal(string.Empty, q);
    }
}
