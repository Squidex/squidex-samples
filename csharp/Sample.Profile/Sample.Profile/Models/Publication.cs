using Newtonsoft.Json;
using Squidex.ClientLibrary;

namespace Sample.Profile.Models
{
    public class Publication : SquidexEntityBase<PublicationData>
    {
    }

    public class PublicationData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string Name { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string Description { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string Link { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string[] Cover { get; set; }
    }
}
