// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

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
        /// <param name="appName">Name of the app. Cannot be null or empty.</param>
        /// <param name="httpClient">The HTTP client. Cannot be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="appName"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="appName"/> is empty.</exception>
        public ExtendableRulesClient(SquidexOptions options, string appName, HttpClient httpClient)
            : base(options, appName, httpClient)
        {
        }

        /// <inheritdoc/>
        public Task<ExtendableRules> GetRulesAsync(
             CancellationToken ct = default)
        {
            return RequestJsonAsync<ExtendableRules>(HttpMethod.Get, BuildUrl(), null, null, ct);
        }

        /// <inheritdoc/>
        public Task<ExtendableRuleDto> CreateRuleAsync(CreateExtendableRuleDto request,
             CancellationToken ct = default)
        {
            Guard.NotNull(request, nameof(request));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Post, BuildUrl(), request.ToContent(), null, ct);
        }

        /// <inheritdoc/>
        public Task<ExtendableRuleDto> UpdateRuleAsync(string id, UpdateExtendableRuleDto request,
             CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));
            Guard.NotNull(request, nameof(request));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{id}"), request.ToContent(), null, ct);
        }

        /// <inheritdoc/>
        public Task<ExtendableRuleDto> EnableRuleAsync(string id,
             CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{id}/enable"), null, null, ct);
        }

        /// <inheritdoc/>
        public Task<ExtendableRuleDto> DisableRuleAsync(string id,
             CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{id}/disable"), null, null, ct);
        }

        /// <inheritdoc/>
        public Task DeleteRuleAsync(string id,
             CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Delete, BuildUrl($"{id}/"), null, null, ct);
        }

        private string BuildUrl(string? path = null)
        {
            return $"api/apps/{AppName}/rules/{path}";
        }
    }
}
