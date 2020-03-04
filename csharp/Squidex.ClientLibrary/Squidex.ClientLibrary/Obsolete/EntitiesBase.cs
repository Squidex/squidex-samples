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
    public abstract class EntitiesBase<T>
    {
        public List<T> Items { get; } = new List<T>();

        public long Total { get; set; }
    }
}
