// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
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
