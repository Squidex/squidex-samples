using Newtonsoft.Json;
using Squidex.ClientLibrary;

namespace Sample.Profile.Models
{
    public class Basics : SquidexEntityBase<BasicsData>
    {
    }

    public class BasicsData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string FirstName { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string LastName { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string Profession { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string Summary { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string[] Image { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string GithubLink { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string BlogLink { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string TwitterLink { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string LinkedInLink { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string EmailAddress { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string LegalTerms { get; set; }
    }
}
