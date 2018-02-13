using Newtonsoft.Json;
using Squidex.ClientLibrary;
using System;

namespace Sample.Profile.Models
{
    public class Education : SquidexEntityBase<EducationData>
    {
    }

    public class EducationData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string Degree { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string School { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string[] Logo { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public DateTime From { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public DateTime? To { get; set; }
    }
}
