// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary;

/// <summary>
/// The result set of rules.
/// </summary>
/// <seealso cref="Resource" />
public sealed class ExtendableRulesDto : Resource
{
    /// <summary>
    /// Gets or sets the rules.
    /// </summary>
    /// <value>
    /// The rules.
    /// </value>
    public List<ExtendableRuleDto> Items { get; set; }
}
