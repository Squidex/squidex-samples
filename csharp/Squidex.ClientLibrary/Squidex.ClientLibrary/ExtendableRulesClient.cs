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
    /// <summary>
    /// The default implementation of the <see cref="IExtendableRulesClient"/> interface.
    /// </summary>
    /// <seealso cref="SquidexClientBase" />
    /// <seealso cref="IExtendableRulesClient" />
    public sealed class ExtendableRulesClient : SquidexClientBase, IExtendableRulesClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendableRulesClient"/> class
        /// with the name of the schema, the options from the <see cref="SquidexClientManager"/> and the HTTP client.
        /// </summary>
        /// <param name="options">The options from the <see cref="SquidexClientManager"/>. Cannot be null.</param>
        /// <param name="httpClient">The HTTP client. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="httpClient"/> is null.</exception>
        public ExtendableRulesClient(SquidexOptions options, HttpClient httpClient)
            : base(options, httpClient)
        {
        }

        /// <inheritdoc/>
        public Task<ExtendableRules> GetRulesAsync(CancellationToken ct = default)
        {
            return RequestJsonAsync<ExtendableRules>(HttpMethod.Get, BuildUrl(), ct: ct);
        }

        /// <inheritdoc/>
        public Task<ExtendableRuleDto> CreateRuleAsync(CreateExtendableRuleDto request, CancellationToken ct = default)
        {
            Guard.NotNull(request, nameof(request));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Post, BuildUrl(), request.ToContent(), ct: ct);
        }

        /// <inheritdoc/>
        public Task<ExtendableRuleDto> UpdateRuleAsync(string id, UpdateExtendableRuleDto request, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));
            Guard.NotNull(request, nameof(request));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{id}"), request.ToContent(), ct: ct);
        }

        /// <inheritdoc/>
        public Task<ExtendableRuleDto> EnableRuleAsync(string id, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{id}/enable"), ct: ct);
        }

        /// <inheritdoc/>
        public Task<ExtendableRuleDto> DisableRuleAsync(string id, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{id}/disable"), ct: ct);
        }

        /// <inheritdoc/>
        public Task DeleteRuleAsync(string id, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Delete, BuildUrl($"{id}/"), ct: ct);
        }

        private string BuildUrl(string? path = null)
        {
            return $"apps/{ApplicationName}/rules/{path}";
        }
    }
}
