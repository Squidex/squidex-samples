// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Squidex.Identity.Model
{
    public abstract class CachingProvider
    {
        private readonly IMemoryCache cache;

        protected CachingProvider(IMemoryCache cache)
        {
            this.cache = cache;
        }

        protected async Task<T> GetOrAddAsync<T>(object key, Func<Task<T>> provider)
        {
            if (!cache.TryGetValue<T>(key, out var result))
            {
                result = await provider();

                cache.Set(key, result, Debugger.IsAttached ? TimeSpan.FromSeconds(1) : TimeSpan.FromMinutes(10));
            }

            return result;
        }
    }
}
