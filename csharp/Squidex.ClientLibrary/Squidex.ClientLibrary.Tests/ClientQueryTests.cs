// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class ClientQueryTests : IClassFixture<ClientQueryFixture>
    {
        public ClientQueryFixture Fixture { get; }

        public ClientQueryTests(ClientQueryFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task Should_return_all()
        {
            var items = await Fixture.Client.GetAsync();

            AssertItems(items, 10, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        }

        [Fact]
        public async Task Should_return_items_with_skip()
        {
            var items = await Fixture.Client.GetAsync(skip: 5);

            AssertItems(items, 10, new[] { 6, 7, 8, 9, 10 });
        }

        [Fact]
        public async Task Should_return_items_with_skip_and_top()
        {
            var items = await Fixture.Client.GetAsync(skip: 2, top: 5);

            AssertItems(items, 10, new[] { 3, 4, 5, 6, 7 });
        }

        [Fact]
        public async Task Should_return_items_with_ordering()
        {
            var items = await Fixture.Client.GetAsync(skip: 2, top: 5, orderBy: "data/value/iv desc");

            AssertItems(items, 10, new[] { 8, 7, 6, 5, 4 });
        }

        [Fact]
        public async Task Should_return_items_with_filter()
        {
            var items = await Fixture.Client.GetAsync(filter: "data/value/iv gt 3 and data/value/iv lt 7");

            AssertItems(items, 3, new[] { 4, 5, 6 });
        }

        private void AssertItems(SquidexEntities<TestEntity, TestEntityData> entities, int total, int[] expected)
        {
            Assert.Equal(total, entities.Total);
            Assert.Equal(expected, entities.Items.Select(x => x.Data.Value).ToArray());
        }
    }
}
