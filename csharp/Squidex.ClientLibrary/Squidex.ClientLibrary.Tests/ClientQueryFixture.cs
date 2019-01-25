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
    public sealed class ClientQueryFixture : IDisposable
    {
        public SquidexClient<TestEntity, TestEntityData> Client { get; } = TestClient.Build();
        public SquidexGlobalClient GlobalClient { get; } = TestClient.BuildGlobalClient();

        public ClientQueryFixture()
        {
            Task.Run(async () =>
            {
                var items = await Client.GetAsync();

                if (items.Total > 10)
                {
                    foreach (var item in items.Items)
                    {
                        await Client.DeleteAsync(item);
                    }
                }

                if (items.Total == 0)
                {
                    for (var i = 10; i > 0; i--)
                    {
                        await Client.CreateAsync(new TestEntityData { Value = i }, true);
                    }
                }
            }).Wait();
        }

        public void Dispose()
        {
        }
    }
}
