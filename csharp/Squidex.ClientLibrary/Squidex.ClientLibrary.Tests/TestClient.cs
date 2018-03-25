namespace Squidex.ClientLibrary.Tests
{
    public static class TestClient
    {
        private static readonly SquidexClientManager clientManager 
            = new SquidexClientManager("https://cloud.squidex.io", "client-test", "client-test:client", "Ify8nZ0O35OyZy6xAwxHFoYw5CcouaYyItPMpk1Df0o=");

        public static SquidexClient<TestEntity, TestEntityData> Build()
        {
            return clientManager.GetClient<TestEntity, TestEntityData>("numbers");
        }
    }
}
