// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;

namespace Squidex.ClientLibrary.Tests
{
    public sealed class QueryFixture : IDisposable
    {
        public SquidexClient<TestEntity, TestEntityData> Client { get; } = TestClient.Build();

        public QueryFixture()
        {
            Task.Run(async () =>
            {
                await TestClient.SetupAsync();

                var contents = await Client.GetAllAsync();

                foreach (var content in contents.Items)
                {
                    await Client.DeleteAsync(content);
                }

                for (var i = 10; i > 0; i--)
                {
                    await Client.CreateAsync(new TestEntityData { Value = i }, true);
                }
            }).Wait();
        }

        public void Dispose()
        {
        }
    }
}
