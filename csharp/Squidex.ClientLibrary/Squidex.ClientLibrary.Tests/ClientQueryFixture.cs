using System;
using System.Threading.Tasks;

namespace Squidex.ClientLibrary.Tests
{
    public sealed class ClientQueryFixture : IDisposable
    {
        public SquidexClient<TestEntity, TestEntityData> Client { get; } = TestClient.Build();

        public ClientQueryFixture()
        {
            Task.Run(async () =>
            {
                var items = await Client.GetAsync();

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
