// ==========================================================================
//  SquidexEntityBase.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary
{
    public abstract class SquidexEntityBase<T> where T : class, new()
    {
        public string Id { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public T Data { get; } = new T();

        internal void MarkAsUpdated()
        {
            LastModified = DateTimeOffset.UtcNow;
        }
    }
}
