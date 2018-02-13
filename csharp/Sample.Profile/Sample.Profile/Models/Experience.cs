using Newtonsoft.Json;
using Squidex.ClientLibrary;
using System;

namespace Sample.Profile.Models
{
    public class Experience : SquidexEntityBase<ExperienceData>
    {
    }

    public class ExperienceData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string Position { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string Company { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string[] Logo { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public DateTime From { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public DateTime? To { get; set; }
    }
}
