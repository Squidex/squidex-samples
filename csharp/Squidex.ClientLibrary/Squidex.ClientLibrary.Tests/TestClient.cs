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
        private static readonly SquidexClientManager ClientManager
            = new SquidexClientManager("https://cloud.squidex.io", "client-test", "client-test:client", "Ify8nZ0O35OyZy6xAwxHFoYw5CcouaYyItPMpk1Df0o=");

        public static SquidexClient<TestEntity, TestEntityData> Build()
        {
            return ClientManager.GetClient<TestEntity, TestEntityData>("numbers");
        }

        public static SquidexGlobalClient BuildGlobalClient()
        {
            return ClientManager.GetGlobalClient();
        }
    }
}
