// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class ManipulationTests : IClassFixture<ManipulationFixture>
    {
        public ManipulationFixture Fixture { get; }

        public ManipulationTests(ManipulationFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task Should_create_strange_text()
        {
            const string text = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36";

            TestEntity item = null;
            try
            {
                item = await Fixture.Client.CreateAsync(new TestEntityData { Text = text }, true);

                var updated = await Fixture.Client.GetAsync(item.Id);

                Assert.Equal(text, updated.Data.Text);
            }
            finally
            {
                await Fixture.Client.DeleteAsync(item.Id);
            }
        }

        [Fact]
        public async Task Should_not_return_not_published_item()
        {
            TestEntity item = null;
            try
            {
                item = await Fixture.Client.CreateAsync(new TestEntityData { Value = 1 });

                await Assert.ThrowsAsync<SquidexException>(() => Fixture.Client.GetAsync(item.Id));
            }
            finally
            {
                await Fixture.Client.DeleteAsync(item.Id);
            }
        }

        [Fact]
        public async Task Should_return_item_published_with_creation()
        {
            TestEntity item = null;
            try
            {
                item = await Fixture.Client.CreateAsync(new TestEntityData { Value = 1 }, true);

                await Fixture.Client.GetAsync(item.Id);
            }
            finally
            {
                await Fixture.Client.DeleteAsync(item.Id);
            }
        }

        [Fact]
        public async Task Should_return_item_published_item()
        {
            TestEntity item = null;
            try
            {
                item = await Fixture.Client.CreateAsync(new TestEntityData { Value = 1 });

                await Fixture.Client.ChangeStatusAsync(item.Id, Status.Published);
                await Fixture.Client.GetAsync(item.Id);
            }
            finally
            {
                await Fixture.Client.DeleteAsync(item.Id);
            }
        }

        [Fact]
        public async Task Should_not_return_archived_item()
        {
            TestEntity item = null;
            try
            {
                item = await Fixture.Client.CreateAsync(new TestEntityData { Value = 1 }, true);

                await Fixture.Client.ChangeStatusAsync(item.Id, Status.Archived);

                await Assert.ThrowsAsync<SquidexException>(() => Fixture.Client.GetAsync(item.Id));
            }
            finally
            {
                await Fixture.Client.DeleteAsync(item.Id);
            }
        }

        [Fact]
        public async Task Should_not_return_unpublished_item()
        {
            TestEntity item = null;
            try
            {
                item = await Fixture.Client.CreateAsync(new TestEntityData { Value = 1 });

                await Fixture.Client.ChangeStatusAsync(item.Id, Status.Published);
                await Fixture.Client.ChangeStatusAsync(item.Id, Status.Draft);

                await Assert.ThrowsAsync<SquidexException>(() => Fixture.Client.GetAsync(item.Id));
            }
            finally
            {
                await Fixture.Client.DeleteAsync(item.Id);
            }
        }

        [Fact]
        public async Task Should_update_item()
        {
            TestEntity item = null;
            try
            {
                item = await Fixture.Client.CreateAsync(new TestEntityData { Value = 2 }, true);

                await Fixture.Client.UpdateAsync(item.Id, new TestEntityData { Value = 2 });

                var updated = await Fixture.Client.GetAsync(item.Id);

                Assert.Equal(2, updated.Data.Value);
            }
            finally
            {
                await Fixture.Client.DeleteAsync(item.Id);
            }
        }

        [Fact]
        public async Task Should_patch_item()
        {
            TestEntity item = null;
            try
            {
                item = await Fixture.Client.CreateAsync(new TestEntityData { Value = 1 }, true);

                await Fixture.Client.PatchAsync(item.Id, new TestEntityData { Value = 2 });

                var updated = await Fixture.Client.GetAsync(item.Id);

                Assert.Equal(2, updated.Data.Value);
            }
            finally
            {
                await Fixture.Client.DeleteAsync(item.Id);
            }
        }
    }
}
