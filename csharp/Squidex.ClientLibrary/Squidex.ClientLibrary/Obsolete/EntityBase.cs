// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Newtonsoft.Json;

namespace Squidex.ClientLibrary
{
    [Obsolete]
    public abstract class EntityBase : Resource
    {
        [JsonProperty("id")]
        public Guid EntityId { get; set; }

        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public bool IsPending { get; set; }

        public int Version { get; set; }

        [JsonIgnore]
        public string Id
        {
            get
            {
                return EntityId.ToString();
            }
            set
            {
                EntityId = Guid.Parse(value);
            }
        }
    }
}
