// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary;

/// <summary>
/// A custom rule client that uses dynamic structures for rule actions.
/// </summary>
/// <remarks>
/// The API and this client library provide concrete types for rule actions.
/// This client library is useful when you connect to a Squidex installation with custom actions that are not known.
/// </remarks>
public interface IExtendableRulesClient
{
    /// <summary>
    /// Query all rules.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The rules.
    /// </returns>
    Task<ExtendableRulesDto> GetRulesAsync(
         CancellationToken ct = default);

    /// <summary>
    /// Creates a new rule.
    /// </summary>
    /// <param name="request">The request to create the rule. Cannot be null.</param>
    /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The created rule.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is null.</exception>
    Task<ExtendableRuleDto> CreateRuleAsync(CreateExtendableRuleDto request,
         CancellationToken ct = default);

    /// <summary>
    /// Updates the rule with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the rule to update. Cannot be null or empty.</param>
    /// <param name="request">The update request. Cannot be null or empty.</param>
    /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The update rule.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
    Task<ExtendableRuleDto> UpdateRuleAsync(string id, UpdateExtendableRuleDto request,
         CancellationToken ct = default);

    /// <summary>
    /// Enables the rule with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the rule to enable. Cannot be null or empty.</param>
    /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The enabled rule.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
    Task<ExtendableRuleDto> EnableRuleAsync(string id,
         CancellationToken ct = default);

    /// <summary>
    /// Disables the rule with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the rule to disable. Cannot be null or empty.</param>
    /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The disabled rule.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
    Task<ExtendableRuleDto> DisableRuleAsync(string id,
         CancellationToken ct = default);

    /// <summary>
    /// Deletes the rule with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the rule to delete. Cannot be null or empty.</param>
    /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The task for completion.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
    Task DeleteRuleAsync(string id,
         CancellationToken ct = default);
}
