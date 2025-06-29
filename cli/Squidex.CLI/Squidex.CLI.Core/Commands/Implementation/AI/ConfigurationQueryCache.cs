// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.CLI.Configuration;

namespace Squidex.CLI.Commands.Implementation.AI;

public sealed class ConfigurationQueryCache(IConfigurationStore configurationStore) : IQueryCache
{
    public Task<GeneratedContent?> GetAsync(string prompt,
        CancellationToken ct = default)
    {
        var result = GetInternal(prompt);
        return Task.FromResult(result);
    }

    public Task StoreAsync(string prompt, GeneratedContent content,
        CancellationToken ct)
    {
        StoreInternal(prompt, content);
        return Task.CompletedTask;
    }

    public GeneratedContent? GetInternal(string prompt)
    {
        var cacheKey = CacheKey(prompt);
        var (cachedResponse, _) = configurationStore.Get<CachedItem>(cacheKey);

        if (string.Equals(cachedResponse?.Prompt, prompt, StringComparison.Ordinal))
        {
            return cachedResponse!.Result;
        }

        return null;
    }

    private void StoreInternal(string prompt, GeneratedContent content)
    {
        var cacheKey = CacheKey(prompt);

        configurationStore.Set(cacheKey, new CachedItem { Prompt = prompt, Result = content });
    }

    private static string CacheKey(string prompt)
    {
        return $"openapi/query-cache/{prompt.ToSha256Base64()}";
    }

    public sealed class CachedItem
    {
        public string Prompt { get; set; }

        public GeneratedContent Result { get; set; }
    }
}
