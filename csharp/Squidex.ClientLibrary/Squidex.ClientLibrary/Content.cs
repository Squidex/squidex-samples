// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.ClientLibrary;

/// <summary>
/// Represents a content item.
/// </summary>
/// <typeparam name="T">The type for the data structure.</typeparam>
/// <seealso cref="Entity" />
public abstract class Content<T> : ContentBase where T : class, new()
{
    /// <summary>
    /// Gets the data of the content item.
    /// </summary>
    /// <value>
    /// The data of the content item. Cannot be replaced.
    /// </value>
    public T Data { get; } = new T();
}

/// <summary>
/// Represents a content item.
/// </summary>
/// <seealso cref="Entity" />
public abstract class ContentBase : Entity
{
    private const string LinkStart = "/api/content/";

    /// <summary>
    /// The new status when this content item has an unpublished, new version.
    /// </summary>
    /// <value>
    /// The new status.
    /// </value>
    public string NewStatus { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the content item.
    /// </summary>
    /// <value>
    /// The status of the content item.
    /// </value>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets current status about the version currently being edited.
    /// </summary>
    /// <value>
    /// The status of the content item.
    /// </value>
    public string EditingStatus => !string.IsNullOrWhiteSpace(NewStatus) ? NewStatus : Status;

    /// <summary>
    /// Gets the name of the app where this content belongs to.
    /// </summary>
    /// <value>
    /// The name of the app where this content belongs to.
    /// </value>
    public string AppName => GetDetails().App;

    /// <summary>
    /// Gets the name of the schema where this content belongs to.
    /// </summary>
    /// <value>
    /// The name of the app schema this content belongs to.
    /// </value>
    public string SchemaName => GetDetails().Schema;

    private (string App, string Schema) GetDetails()
    {
        if (Links == null || !Links.TryGetValue("self", out var self))
        {
            throw new InvalidOperationException("Content has no self link.");
        }

        try
        {
            var index = self.Href.IndexOf(LinkStart, StringComparison.Ordinal);

#pragma warning disable IDE0057 // Use range operator
            var href = self.Href.Substring(index + LinkStart.Length);
#pragma warning restore IDE0057 // Use range operator
            var hrefp = href.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            return (hrefp[0], hrefp[1]);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Link {self.Href} is malformed.", ex);
        }
    }
}
