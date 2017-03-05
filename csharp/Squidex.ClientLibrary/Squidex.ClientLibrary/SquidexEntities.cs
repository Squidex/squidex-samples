// ==========================================================================
//  SquidexEntities.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Collections.Generic;

namespace Squidex.ClientLibrary
{
    public sealed class SquidexEntities<TEntity, TData> where TData : class, new() where TEntity : SquidexEntityBase<TData>
    {
        public List<TEntity> Items { get; } = new List<TEntity>();

        public long Total { get; set; }
    }
}
