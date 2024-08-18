// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.ClientLibrary;

/// <summary>
/// The request to change a status.
/// </summary>
public sealed record ChangeStatus
{
    /// <summary>
    /// The new status.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// The due time.
    /// </summary>
    public DateTime? DueTime { get; set; }

    /// <summary>
    /// True to check referrers of this content.
    /// </summary>
    public bool CheckReferrers { get; set; }
}

/// <summary>
/// The options for creating content items.
/// </summary>
public record struct ContentCreateOptions
{
    /// <summary>
    /// The optional custom ID for the content item.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// If set to <c>true</c> the content will be published.
    /// </summary>
    public bool Publish { get; set; }

    /// <summary>
    /// A option where <see cref="Publish"/> is true.
    /// </summary>
    public static readonly ContentCreateOptions AsPublish = new ContentCreateOptions
    {
        Publish = true
    };
}

/// <summary>
/// The options for upserting content items.
/// </summary>
public record struct ContentUpsertOptions
{
    /// <summary>
    /// If set to <c>true</c> the content will be published.
    /// </summary>
    public bool Publish { get; set; }

    /// <summary>
    /// Makes the update as patch.
    /// </summary>
    public bool Patch { get; set; }

    /// <summary>
    /// Enrich the content with defaults.
    /// </summary>
    public bool EnrichDefaults { get; set; }

    /// <summary>
    /// A option where <see cref="Publish"/> is true.
    /// </summary>
    public static readonly ContentUpsertOptions AsPublish = new ContentUpsertOptions
    {
        Publish = true
    };

    /// <summary>
    /// A option where <see cref="Patch"/> is true.
    /// </summary>
    public static readonly ContentUpsertOptions AsPatch = new ContentUpsertOptions
    {
        Patch = true
    };

    /// <summary>
    /// A option where <see cref="EnrichDefaults"/> is true.
    /// </summary>
    public static readonly ContentUpsertOptions WithEnrichedDefaults = new ContentUpsertOptions
    {
        EnrichDefaults = true
    };
}

/// <summary>
/// The options for deleting content items.
/// </summary>
public record struct ContentDeleteOptions
{
    /// <summary>
     /// True to delete the content permanently.
     /// </summary>
    public bool Permanent { get; set; }

    /// <summary>
    /// True to check referrers of this content.
    /// </summary>
    public bool CheckReferrers { get; set; }

    /// <summary>
    /// A option where <see cref="Permanent"/> is true.
    /// </summary>
    public static readonly ContentDeleteOptions AsPermanent = new ContentDeleteOptions
    {
        Permanent = true
    };
}

/// <summary>
/// The options for updating content items. Used for later.
/// </summary>
public record struct ContentUpdateOptions
{
    /// <summary>
    /// Enrich the content with defaults.
    /// </summary>
    public bool EnrichDefaults { get; set; }

    /// <summary>
    /// A option where <see cref="EnrichDefaults"/> is true.
    /// </summary>
    public static readonly ContentUpdateOptions WithEnrichedDefaults = new ContentUpdateOptions
    {
        EnrichDefaults = true
    };
}

/// <summary>
/// The options for creating content drafts. Used for later.
/// </summary>
public record struct ContentCreateDraftOptions
{
}

/// <summary>
/// The options for deleting content drafts. Used for later.
/// </summary>
public record struct ContentDeleteDraftOptions
{
}

/// <summary>
/// The options for patching content items. Used for later.
/// </summary>
public record struct ContentPatchOptions
{
}

/// <summary>
/// The options for enriching content items. Used for later.
/// </summary>
public record struct ContentEnrichDefaultsOptions
{
    /// <summary>
    ///  True, to also enrich required fields. Default: false.
    /// </summary>
    public bool EnrichRequiredFields { get; set; }

    /// <summary>
    /// A option where <see cref="EnrichRequiredFields"/> is true.
    /// </summary>
    public static readonly ContentEnrichDefaultsOptions WithRequiredFields = new ContentEnrichDefaultsOptions
    {
        EnrichRequiredFields = true
    };
}
