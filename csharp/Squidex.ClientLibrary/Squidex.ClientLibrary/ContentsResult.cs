// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

namespace Squidex.ClientLibrary
{
    public sealed class ContentsResult<TEntity, TData> : Resource where TEntity : Content<TData> where TData : class, new()
    {
        public List<TEntity> Items { get; } = new List<TEntity>();

        public long Total { get; set; }
    }
}
