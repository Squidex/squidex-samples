// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary;

/// <summary>
/// The default implementation of the <see cref="IExtendableRulesClient"/> interface.
/// </summary>
/// <seealso cref="SquidexClientBase" />
/// <seealso cref="IExtendableRulesClient" />
/// <remarks>
/// Initializes a new instance of the <see cref="ExtendableRulesClient"/> class
/// with the name of the schema, the options from the <see cref="SquidexClient"/> and the HTTP client.
/// </remarks>
/// <param name="options">The options from the <see cref="SquidexClient"/>. Cannot be null.</param>
/// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
public sealed class ExtendableRulesClient(SquidexOptions options) : SquidexClientBase(options), IExtendableRulesClient
{
    /// <inheritdoc/>
    public Task<ExtendableRulesDto> GetRulesAsync(
         CancellationToken ct = default)
    {
        return RequestJsonAsync<ExtendableRulesDto>(HttpMethod.Get, BuildUrl(), null, null, ct);
    }

    /// <inheritdoc/>
    public Task<ExtendableRuleDto> CreateRuleAsync(CreateExtendableRuleDto request,
         CancellationToken ct = default)
    {
        Guard.NotNull(request, nameof(request));

        return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Post, BuildUrl(), request.ToContent(Options), null, ct);
    }

    /// <inheritdoc/>
    public Task<ExtendableRuleDto> UpdateRuleAsync(string id, UpdateExtendableRuleDto request,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));
        Guard.NotNull(request, nameof(request));

        return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{Uri.EscapeDataString(id)}"), request.ToContent(Options), null, ct);
    }

    /// <inheritdoc/>
    public Task<ExtendableRuleDto> EnableRuleAsync(string id,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));

        return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{Uri.EscapeDataString(id)}/enable"), null, null, ct);
    }

    /// <inheritdoc/>
    public Task<ExtendableRuleDto> DisableRuleAsync(string id,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));

        return RequestJsonAsync<ExtendableRuleDto>(HttpMethod.Put, BuildUrl($"{Uri.EscapeDataString(id)}/disable"), null, null, ct);
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
        return $"api/apps/{Options.AppName}/rules/{path}";
    }
}
