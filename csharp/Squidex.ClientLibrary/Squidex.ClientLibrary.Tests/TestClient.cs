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
            = new SquidexClientManager("http://localhost:5000", "testing-app", "testing-app:server", "MUKDxdgAVx9LufFrcO5T1pMtZM137climipDCixitig=");

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
