// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.Configuration;

internal sealed class Cache<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, CacheEntry> items = new Dictionary<TKey, CacheEntry>();

    private sealed class CacheEntry
    {
        public TValue Value { get; set; }

        public DateTimeOffset Expires { get; set; }
    }

    public bool Remove(TKey key)
    {
        lock (items)
        {
            Cleanup();

            return items.Remove(key);
        }
    }

    public void Set(TKey key, TValue value, DateTimeOffset expires)
    {
        lock (items)
        {
            Cleanup();

            items[key] = new CacheEntry { Value = value, Expires = expires };
        }
    }

    public bool TryGet(TKey key, out TValue value)
    {
        lock (items)
        {
            Cleanup();

            value = default!;

            if (items.TryGetValue(key, out var entry))
            {
                value = entry.Value;
                return true;
            }

            return false;
        }
    }

    public void Set(TKey key, TValue value, TimeSpan expires)
    {
        if (expires < TimeSpan.Zero || expires > TimeSpan.FromDays(365))
        {
            return;
        }

        Set(key, value, DateTime.UtcNow + expires);
    }

    private void Cleanup()
    {
        if (items.Count == 0)
        {
            return;
        }

        List<TKey>? keysToRemove = null;

        var now = DateTime.UtcNow;

        foreach (var kvp in items)
        {
            if (kvp.Value.Expires <= now)
            {
                keysToRemove ??= new List<TKey>();
                keysToRemove.Add(kvp.Key);
            }
        }

        if (keysToRemove == null)
        {
            return;
        }

        foreach (var key in keysToRemove)
        {
            items.Remove(key);
        }
    }
}
