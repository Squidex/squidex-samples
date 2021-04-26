// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Newtonsoft.Json;
using Squidex.ClientLibrary;

namespace Sample.Profile.Models
{
    public class Experience : Content<ExperienceData>
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
