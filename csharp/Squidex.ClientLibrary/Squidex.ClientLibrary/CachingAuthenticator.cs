// ==========================================================================
//  InvariantConverter.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Squidex.ClientLibrary
{
    public class CachingAuthenticator : Authenticator
    {
        private readonly IMemoryCache cache;
        private readonly string cacheKey;

        public CachingAuthenticator(IMemoryCache cache, Uri serviceUrl, string clientId, string clientSecret) 
            : base(serviceUrl, clientId, clientSecret)
        {
            this.cache = cache;

            cacheKey = $"{serviceUrl}_TOKEN";
        }

        public CachingAuthenticator(Uri serviceUrl, string clientId, string clientSecret)
            : this(new MemoryCache(Options.Create(new MemoryCacheOptions())), serviceUrl, clientId, clientSecret)
        {
        }

        protected override string GetFromCache()
        {
            return cache.Get<string>(cacheKey);
        }

        protected override void SetToCache(string result, DateTimeOffset expires)
        {
            cache.Set(cacheKey, result, expires);
        }
    }
}
