// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Squidex.ClientLibrary;

namespace Squidex.Identity.Model
{
    public sealed class CachedSettingsProvider : ISettingsProvider
    {
        private readonly SquidexClient<SquidexSettings, SquidexSettingsData> apiClient;
        private readonly IMemoryCache cache;
        private readonly IOptions<SquidexSettingsData> defaults;

        public CachedSettingsProvider(SquidexClientManager clientManager, IMemoryCache cache, IOptions<SquidexSettingsData> defaults)
        {
            apiClient = clientManager.GetClient<SquidexSettings, SquidexSettingsData>("settings");

            this.cache = cache;
            this.defaults = defaults;
        }

        public async Task<SquidexSettingsData> GetSettingsAsync()
        {
            if (!cache.TryGetValue<SquidexSettingsData>("Settings", out var result))
            {
                var settings = await apiClient.GetAsync();

                result = settings.Items.FirstOrDefault()?.Data ?? new SquidexSettingsData();

                foreach (var property in result.GetType().GetProperties())
                {
                    if (property.GetValue(result) == null)
                    {
                        property.SetValue(result, property.GetValue(defaults.Value));
                    }
                }

                cache.Set("Settings", result, TimeSpan.FromMinutes(10));
            }

            return result;
        }
    }
}
