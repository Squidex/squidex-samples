// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.Tests
{
    public static class TestClient
    {
        public static readonly SquidexClientManager ClientManager
            = new SquidexClientManager("http://localhost:5000", "hello", "hello:default", "xeLd6jFxqbXJrfmNLlO2j1apagGGGSyZJhFnIuHp4I0=");

        public static SquidexClient<TestEntity, TestEntityData> Build()
        {
            return ClientManager.GetClient<TestEntity, TestEntityData>("numbers");
        }
    }
}
