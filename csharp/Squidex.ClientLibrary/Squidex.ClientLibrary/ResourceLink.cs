// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary;

/// <summary>
/// Represents a link of a resource.
/// </summary>
public class ResourceLink
{
    /// <summary>
    /// Gets or sets the URL to the link.
    /// </summary>
    /// <value>
    /// The URL to the link.
    /// </value>
    public string Href { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method of the endpoint where the link points to.
    /// </summary>
    /// <value>
    /// The HTTP method of the endpoint where the link points to.
    /// </value>
    public string Method { get; set; }

    /// <summary>
    /// Gets or sets the additional metadata for the link.
    /// </summary>
    /// <value>
    /// The additional metadata for the link.
    /// </value>
    public string Metadata { get; set; }
}
