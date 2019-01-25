﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;
using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class ManagementTests
    {
        private readonly ISchemasClient schemasClient;

        public ManagementTests()
        {
            schemasClient = TestClient.ClientManager.CreateSchemasClient();
        }

        [Fact]
        public async Task Should_query_schemas()
        {
            var schemas = await schemasClient.GetSchemasAsync(TestClient.ClientManager.App);

            Assert.NotEmpty(schemas);
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
                Publish = true,
                Properties = new SchemaPropertiesDto
                {
                    Label = "New Schema"
                },
                Fields = new List<CreateSchemaFieldDto>
                {
                    new CreateSchemaFieldDto
                    {
                        Name = "String",
                        Properties = new StringFieldPropertiesDto
                        {
                            IsRequired = true
                        }
                    }
                }
            });
        }
    }
}
