// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary
{
    public abstract class SquidexEntityBase<T> : EntityBase where T : class, new()
    {
        public T Data { get; } = new T();

        public T DataDraft { get; set; }

        internal void MarkAsUpdated()
        {
            LastModified = DateTimeOffset.UtcNow;
        }
    }
}
