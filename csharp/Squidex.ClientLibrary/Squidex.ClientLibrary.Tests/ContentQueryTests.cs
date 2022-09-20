// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class ContentQueryTests
    {
        [Fact]
        public void Should_create_query_for_empty_query()
        {
            var query = new ContentQuery().ToQuery(true);

            Assert.Empty(query);
        }

        [Fact]
        public void Should_create_query_for_top()
        {
            var query = new ContentQuery { Top = 10 }.ToQuery(true);

            Assert.Equal("?$top=10", query);
        }

        [Fact]
        public void Should_create_query_for_skip()
        {
            var query = new ContentQuery { Skip = 10 }.ToQuery(true);

            Assert.Equal("?$skip=10", query);
        }

        [Fact]
        public void Should_create_query_for_skip_and_top()
        {
            var query = new ContentQuery { Skip = 20, Top = 10 }.ToQuery(true);

            Assert.Equal("?$skip=20&$top=10", query);
        }

        [Fact]
        public void Should_create_query_for_filter()
        {
            var query = new ContentQuery { Filter = "my-filter" }.ToQuery(true);

            Assert.Equal("?$filter=my-filter", query);
        }

        [Fact]
        public void Should_create_query_for_search()
        {
            var query = new ContentQuery { Search = "my-search" }.ToQuery(true);

            Assert.Equal("?$search=\"my-search\"", query);
        }

        [Fact]
        public void Should_create_query_for_search_and_filter()
        {
            var query = new ContentQuery { Search = "my-search", Filter = "my-filter" }.ToQuery(true);

            Assert.Equal("?$search=\"my-search\"&$filter=my-filter", query);
        }
    }
}
