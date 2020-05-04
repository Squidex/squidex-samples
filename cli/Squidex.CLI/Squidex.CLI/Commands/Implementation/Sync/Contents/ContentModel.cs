// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema.Annotations;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public sealed class ContentModel
    {
        [Required]
        public string Schema { get; set; }

        [Required]
        public JObject Data { get; set; }

        public JObject Filter { get; set; }

        public Dictionary<string, ContentReference> References { get; set; }

        [JsonSchemaIgnore]
        [JsonIgnore]
        public string File { get; set; }

        [JsonSchemaIgnore]
        [JsonIgnore]
        public string Ref { get; set; }
    }
}
