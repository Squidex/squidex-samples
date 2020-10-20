// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary
{
    public sealed class ExtendableRulesClient : SquidexClientBase, IExtendableRulesClient
    {
        public ExtendableRulesClient(SquidexOptions options, HttpClient httpClient)
            : base(options, httpClient)
        {
        }

        public Task<ExtendableRules> GetRulesAsync(CancellationToken ct = default)
        {
            return RequestJsonAsync<ExtendableRules>(HttpMethod.Get, BuildUrl(), ct: ct);
        }

        public Task<ExtendableRuleDto> CreateRuleAsync(CreateExtendableRuleDto request, CancellationToken ct = default)
        {
            Guard.NotNull(request, nameof(request));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Post, BuildUrl(), request.ToContent(), ct: ct);
        }

        public Task<ExtendableRuleDto> UpdateRuleAsync(string id, UpdateExtendableRuleDto request, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));
            Guard.NotNull(request, nameof(request));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{id}"), request.ToContent(), ct: ct);
        }

        public Task<ExtendableRuleDto> EnableRuleAsync(string id, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{id}/enable"), ct: ct);
        }

        public Task<ExtendableRuleDto> DisableRuleAsync(string id, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{id}/disable"), ct: ct);
        }

        public Task DeleteRuleAsync(string id, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Delete, BuildUrl($"{id}/"), ct: ct);
        }

        private string BuildUrl(string path = null)
        {
            return $"apps/{ApplicationName}/rules/{path}";
        }
    }
}
