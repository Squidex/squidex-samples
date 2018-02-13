using Newtonsoft.Json;
using Squidex.ClientLibrary;

namespace Sample.Profile.Models
{
    public class Skill : SquidexEntityBase<SkillData>
    {
    }

    public class SkillData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string Name { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string Experience { get; set; }
    }
}
