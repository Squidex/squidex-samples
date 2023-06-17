// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json.Linq;

namespace Squidex.ClientLibrary;

/// <summary>
/// A HTTP request to create a rule.
/// </summary>
public sealed class CreateExtendableRuleDto
{
    /// <summary>
    /// Gets or sets the optional name of the rule.
    /// </summary>
    /// <value>
    /// The optional name of the rule.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the rule trigger.
    /// </summary>
    /// <value>
    /// The rule trigger.
    /// </value>
    public RuleTriggerDto Trigger { get; set; }

    /// <summary>
    /// Gets or sets the rule action.
    /// </summary>
    /// <value>
    /// The rule action.
    /// </value>
    public JObject Action { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this rule is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this rule is enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsEnabled { get; set; }
}
