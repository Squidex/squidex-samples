// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.ClientLibrary;

/// <summary>
/// Represents a resource, which is an entity or collection of entities that are delivered by the API.
/// </summary>
public abstract class Resource
{
    /// <summary>
    /// Gets the links of this resource.
    /// </summary>
    /// <value>
    /// The links of this resource.
    /// </value>
    [JsonProperty("_links")]
    public Dictionary<string, ResourceLink> Links { get; } = new Dictionary<string, ResourceLink>();
}
