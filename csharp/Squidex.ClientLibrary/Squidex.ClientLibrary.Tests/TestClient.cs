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
        public const string AppName = "integration-tests";
        public const string ClientId = "root";
        public const string ClientSecret = "xeLd6jFxqbXJrfmNLlO2j1apagGGGSyZJhFnIuHp4I0=";
        public const string SchemaName = "numbers";
        public const string ServerUrl = "http://localhost:5000";
        public const string FieldName = "value";

        public static readonly SquidexClientManager ClientManager =
            new SquidexClientManager("http://localhost:5000", AppName, ClientId, ClientSecret)
            {
                ReadResponseAsString = true
            };

        public static SquidexClient<TestEntity, TestEntityData> Build()
        {
            return ClientManager.GetClient<TestEntity, TestEntityData>(SchemaName);
        }
    }
}
