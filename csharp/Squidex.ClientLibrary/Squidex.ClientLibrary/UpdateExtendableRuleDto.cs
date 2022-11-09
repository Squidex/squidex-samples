// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary.Management;

namespace Squidex.ClientLibrary;

/// <summary>
/// A HTTP request to update a rule.
/// </summary>
public sealed class UpdateExtendableRuleDto
{
    /// <summary>
    /// Gets or sets the optional new name of the rule.
    /// </summary>
    /// <value>
    /// The optional new name of the rule.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the optional new trigger.
    /// </summary>
    /// <value>
    /// The optional new trigger.
    /// </value>
    public RuleTriggerDto Trigger { get; set; }

    /// <summary>
    /// Gets or sets the optional new action.
    /// </summary>
    /// <value>
    /// The optional new action.
    /// </value>
    public JObject Action { get; set; }
}