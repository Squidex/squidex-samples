using Newtonsoft.Json;

namespace Squidex.ClientLibrary.Tests
{
    public sealed class TestEntity : SquidexEntityBase<TestEntityData>
    {
    }

    public sealed class TestEntityData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public int Value { get; set; }
    }
}
