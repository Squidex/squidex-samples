// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

namespace Squidex.ClientLibrary
{
    public abstract class EntitiesBase<T>
    {
        public List<T> Items { get; } = new List<T>();

        public long Total { get; set; }
    }
}
