// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Squidex.ClientLibrary;

namespace Squidex.Identity.Model
{
    public sealed class SettingsProvider : CachingProvider, ISettingsProvider
    {
        private readonly SquidexClient<SettingsEntity, SettingsData> apiClient;
        private readonly IOptions<SettingsData> defaults;

        public SettingsProvider(SquidexClientManager clientManager, IMemoryCache cache, IOptions<SettingsData> defaults)
            : base(cache)
        {
            apiClient = clientManager.GetClient<SettingsEntity, SettingsData>("settings");

            this.defaults = defaults;
        }

        public Task<SettingsData> GetSettingsAsync()
        {
            return GetOrAddAsync(nameof(SettingsProvider), async () =>
            {
                var settings = await apiClient.GetAsync();

                var result = settings.Items.FirstOrDefault()?.Data ?? new SettingsData();

                foreach (var property in result.GetType().GetProperties())
                {
                    if (property.GetValue(result) == null)
                    {
                        property.SetValue(result, property.GetValue(defaults.Value));
                    }
                }

                return result;
            });
        }
    }
}
