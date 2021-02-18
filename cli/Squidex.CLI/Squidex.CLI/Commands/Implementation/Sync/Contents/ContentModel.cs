// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using NJsonSchema.Annotations;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public sealed class ContentModel
    {
        [JsonSchemaIgnore]
        [JsonIgnore]
        public string File { get; set; }

        [JsonSchemaIgnore]
        [JsonIgnore]
        public string Ref { get; set; }

        [Required]
        public string Schema { get; set; }

        [Required]
        public DynamicData Data { get; set; }

        public string Id { get; set; }

        public string Status { get; set; }
    }
}
