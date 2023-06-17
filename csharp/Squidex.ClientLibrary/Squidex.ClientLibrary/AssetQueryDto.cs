// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Utils;

#pragma warning disable SA1629 // Documentation text should end with a period

namespace Squidex.ClientLibrary;

/// <summary>
/// Represents an asset query.
/// </summary>
public sealed class AssetQuery
{
    /// <summary>
    /// Gets or sets the IDs of the assets to retrieve.
    /// </summary>
    /// <value>
    /// The IDs of the assets to retrieve.
    /// </value>
    /// <remarks>
    /// If this option is provided all other properties are ignored.
    /// </remarks>
    public List<string>? Ids { get; set; }

    /// <summary>
    /// Gets or sets the JSON query.
    /// </summary>
    /// <value>
    /// The JSON query.
    /// </value>
    /// <remarks>
    /// Do not use this property in combination with OData properties.
    /// </remarks>
    public object? Query { get; set; }

    /// <summary>
    /// Gets or sets the OData argument to define the number of assets to retrieve (<code>$top</code>).
    /// </summary>
    /// <value>
    /// The the number of assets to retrieve.
    /// </value>
    /// <remarks>
    /// Use this property to implement pagination but not in combination with <see cref="Query"/> property.
    /// </remarks>
    public int? Top { get; set; }

    /// <summary>
    /// Gets or sets the OData argument to define number of assets to skip (<code>$skip</code>).
    /// </summary>
    /// <value>
    /// The the number of assets to skip.
    /// </value>
    /// <remarks>
    /// Use this property to implement pagination but not in combination with <see cref="Query"/> property.
    /// </remarks>
    public int? Skip { get; set; }

    /// <summary>
    /// Gets or sets the OData order argument (<code>$orderby</code>).
    /// </summary>
    /// <value>
    /// The OData order argument.
    /// </value>
    /// <remarks>
    /// Do not use this property in combination with <see cref="Query"/> property.
    /// </remarks>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Gets or sets the OData filter argument (<code>$filter</code>).
    /// </summary>
    /// <value>
    /// The OData filter argument.
    /// </value>
    /// <remarks>
    /// Do not use this property in combination with <see cref="Query"/> property.
    /// </remarks>
    public string? Filter { get; set; }

    /// <summary>
    /// Gets or sets the optional folder ID.
    /// </summary>
    /// <value>
    /// The parent optional folder ID.
    /// </value>
    public string? ParentId { get; set; }

    internal string? ToQueryJson()
    {
        return Query?.ToJson();
    }

    internal string? ToIdString()
    {
        return Ids == null ? null : string.Join(",", Ids);
    }
}
