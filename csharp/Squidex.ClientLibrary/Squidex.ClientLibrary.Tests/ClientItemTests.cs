// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using Squidex.ClientLibrary.Transfer;
using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class ClientItemTests : IClassFixture<ClientQueryFixture>
    {
        public ClientQueryFixture Fixture { get; }

        public ClientItemTests(ClientQueryFixture fixture)
        {
            Fixture = fixture;
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

                await Fixture.Client.PublishAsync(item.Id);
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

                await Fixture.Client.RestoreAsync(item.Id);

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

                await Fixture.Client.PublishAsync(item.Id);
                await Fixture.Client.UnpublishAsync(item.Id);

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
                item = await Fixture.Client.CreateAsync(new TestEntityData { Value = 1 }, true);

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
        public async Task Should_return_application_users()
        {
            var res = await Fixture.GlobalClient.GetAllUsers();
        }

        [Fact]
        public async Task Should_create_a_new_application()
        {
            CreateAppRequest request = new CreateAppRequest();
            request.Name = "testblakeapp";
            request.Template = string.Empty;

            await Fixture.GlobalClient.CreateApp(request);
        }

        [Fact]
        public async Task Should_add_a_new_contributor()
        {
            await Fixture.GlobalClient.AddContributor();
        }

        [Fact]
        public async Task Should_add_a_new_client()
        {
            CreateClientRequest request = new CreateClientRequest();
            request.Id = "chechs";
            var res = await Fixture.GlobalClient.CreateClientCredentials(request);
        }
    }
}
