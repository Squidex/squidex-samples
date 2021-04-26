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
    public class Education : Content<EducationData>
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
