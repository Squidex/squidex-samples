// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;
using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class ManagementTests
    {
        private readonly ISchemasClient schemasClient;
        private readonly IAppsClient appsClient;

        public ManagementTests()
        {
            schemasClient = TestClient.ClientManager.CreateSchemasClient();

            appsClient = TestClient.ClientManager.CreateAppsClient();
        }

        [Fact]
        public async Task Should_query_apps()
        {
            await appsClient.PostAppAsync(new CreateAppDto { Name = "temporary" });

            var apps = await appsClient.GetAppsAsync();

            Assert.Equal("temporary", apps.FirstOrDefault()?.Name);
        }

        [Fact]
        public async Task Should_query_contributors()
        {
            var contributors = await appsClient.GetContributorsAsync(TestClient.ClientManager.App);

            Assert.NotEmpty(contributors.Items);
        }

        [Fact]
        public async Task Should_query_schemas()
        {
            var schemas = await schemasClient.GetSchemasAsync(TestClient.ClientManager.App);

            Assert.NotEmpty(schemas.Items);
        }

        [Fact]
        public async Task Should_query_schema()
        {
            var schema = await schemasClient.GetSchemaAsync(TestClient.ClientManager.App, "numbers");

            Assert.NotNull(schema);
        }

        [Fact]
        public async Task Should_create_schema()
        {
            await schemasClient.PostSchemaAsync(TestClient.ClientManager.App, new CreateSchemaDto
            {
                Name = "new-schema",
                Properties = new SchemaPropertiesDto
                {
                    Label = "New Schema"
                },
                Fields = new List<UpsertSchemaFieldDto>
                {
                    new UpsertSchemaFieldDto
                    {
                        Name = "String",
                        Properties = new StringFieldPropertiesDto
                        {
                            IsRequired = true
                        }
                    }
                },
                IsPublished = true
            });
        }
    }
}
