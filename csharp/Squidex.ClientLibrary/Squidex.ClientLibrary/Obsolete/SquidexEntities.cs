// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Squidex.ClientLibrary
{
    [Obsolete("Use Squidex.ClientLibrary.ContentsResult instead.")]
    public sealed class SquidexEntities<TEntity, TData> : Resource where TEntity : SquidexEntityBase<TData> where TData : class, new()
    {
        public List<TEntity> Items { get; } = new List<TEntity>();

        public long Total { get; set; }
    }
}
