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
    public sealed class ManipulationFixture : IDisposable
    {
        public SquidexClient<TestEntity, TestEntityData> Client { get; } = TestClient.Build();

        public ManipulationFixture()
        {
            Task.Run(async () =>
            {
                await TestClient.SetupAsync();
            }).Wait();
        }

        public void Dispose()
        {
        }
    }
}
