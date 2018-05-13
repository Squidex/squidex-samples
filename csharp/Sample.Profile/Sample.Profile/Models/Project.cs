// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Squidex.ClientLibrary;

namespace Sample.Profile.Models
{
    public class Project : SquidexEntityBase<ProjectData>
    {
    }

    public class ProjectData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string Name { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string Label { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string Link { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string Description { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public string[] Image { get; set; }

        [JsonConverter(typeof(InvariantConverter))]
        public int Year { get; set; }
    }
}
