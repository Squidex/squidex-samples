// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

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
    }
}
