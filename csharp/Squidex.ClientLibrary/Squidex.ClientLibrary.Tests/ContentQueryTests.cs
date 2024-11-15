// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.Tests;

public class ContentQueryTests
{
    private readonly SquidexOptions options = new SquidexOptions();

    [Fact]
    public void ContentQuery_as_default()
    {
        var query = new ContentQuery()
            .ToQuery(true, options)
            .ToString();

        Assert.Equal(string.Empty, query);
    }

    [Fact]
    public void ContentQuery_with_top()
    {
        var query = new ContentQuery { Top = 10 }
            .ToQuery(true, options)
            .ToString();

        Assert.Equal("?$top=10", query);
    }

    [Fact]
    public void ContentQuery_with_skip()
    {
        var query = new ContentQuery { Skip = 10 }
            .ToQuery(true, options)
            .ToString();

        Assert.Equal("?$skip=10", query);
    }

    [Fact]
    public void ContentQuery_with_skip_and_top()
    {
        var query = new ContentQuery { Skip = 20, Top = 10 }
            .ToQuery(true, options)
            .ToString();

        Assert.Equal("?$skip=20&$top=10", query);
    }

    [Fact]
    public void ContentQuery_with_filter()
    {
        var query = new ContentQuery { Filter = "my-filter" }
            .ToQuery(true, options)
            .ToString();

        Assert.Equal("?$filter=my-filter", query);
    }

    [Fact]
    public void ContentQuery_with_search()
    {
        var query = new ContentQuery { Search = "my-search" }
            .ToQuery(true, options)
            .ToString();

        Assert.Equal("?$search=\"my-search\"", query);
    }

    [Fact]
    public void ContentQuery_with_search_and_filter()
    {
        var query = new ContentQuery { Search = "my-search", Filter = "my-filter" }
            .ToQuery(true, options)
            .ToString();

        Assert.Equal("?$filter=my-filter&$search=\"my-search\"", query);
    }

    [Fact]
    public void ContentQuery_with_random()
    {
        var query = new ContentQuery { Random = 42 }
            .ToQuery(true, options)
            .ToString();

        Assert.Equal("?random=42", query);
    }

    [Fact]
    public void ContentQuery_with_collation()
    {
        var query = new ContentQuery { Collation = "tr" }
            .ToQuery(true, options)
            .ToString();

        Assert.Equal("?collation=tr", query);
    }

    [Fact]
    public void ContentQuery_with_ids()
    {
        var query = new ContentQuery { Ids = ["1", "2", "3"] }
            .ToQuery(true, options)
            .ToString();

        Assert.Equal("?ids=1%2C2%2C3", query);
    }

    [Fact]
    public void ContentCreateOptions_as_default()
    {
        var query = default(ContentCreateOptions)
            .ToQuery()
            .ToString();

        Assert.Equal(string.Empty, query);
    }

    [Fact]
    public void ContentCreateOptions_with_everything()
    {
        var query = new ContentCreateOptions { Id = "1", Publish = true }
            .ToQuery()
            .ToString();

        Assert.Equal("?id=1&publish=true", query);
    }

    [Fact]
    public void ContentUpsertOptions_as_default()
    {
        var query = default(ContentUpsertOptions)
            .ToQuery()
            .ToString();

        Assert.Equal(string.Empty, query);
    }

    [Fact]
    public void ContentUpsertOptions_with_everything()
    {
        var query = new ContentUpsertOptions { Publish = true, Patch = true, EnrichDefaults = true, EnrichRequiredFields = true }
            .ToQuery()
            .ToString();

        Assert.Equal("?enrichDefaults=true&enrichRequiredFields=true&patch=true&publish=true", query);
    }

    [Fact]
    public void ContentDeleteOptions_as_default()
    {
        var query = default(ContentDeleteOptions)
            .ToQuery()
            .ToString();

        Assert.Equal(string.Empty, query);
    }

    [Fact]
    public void ContentDeleteOptions_with_everything()
    {
        var query = new ContentDeleteOptions { Permanent = true }
            .ToQuery()
            .ToString();

        Assert.Equal("?permanent=true", query);
    }

    [Fact]
    public void ContentUpdateOptions_as_default()
    {
        var query = default(ContentUpdateOptions)
            .ToQuery()
            .ToString();

        Assert.Equal(string.Empty, query);
    }

    [Fact]
    public void ContentUpdateOptions_with_everything()
    {
        var query = new ContentUpdateOptions { EnrichDefaults = true }
            .ToQuery()
            .ToString();

        Assert.Equal("?enrichDefaults=true", query);
    }

    [Fact]
    public void ContentEnrichDefaultsOptions_as_default()
    {
        var query = default(ContentEnrichDefaultsOptions)
            .ToQuery()
            .ToString();

        Assert.Equal(string.Empty, query);
    }

    [Fact]
    public void ContentEnrichDefaultsOptions_with_everything()
    {
        var query = new ContentEnrichDefaultsOptions { EnrichRequiredFields = true }
            .ToQuery()
            .ToString();

        Assert.Equal("?enrichRequiredFields=true", query);
    }
}
