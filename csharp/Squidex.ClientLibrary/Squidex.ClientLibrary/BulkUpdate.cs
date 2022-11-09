// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary;

/// <summary>
/// Represents a bulk update.
/// </summary>
public sealed class BulkUpdate
{
    /// <summary>
    /// The contents to update or insert.
    /// </summary>
    public List<BulkUpdateJob> Jobs { get; set; } = new List<BulkUpdateJob>();

    /// <summary>
    /// True to automatically publish the content.
    /// </summary>
    public bool Publish { get; set; }

    /// <summary>
    /// True to turn off scripting for faster inserts. Default: true.
    /// </summary>
    public bool DoNotScript { get; set; } = true;

    /// <summary>
    /// True to turn off validation for faster inserts. Default: false.
    /// </summary>
    public bool DoNotValidate { get; set; }

    /// <summary>
    /// True to turn off validation of workflow rules. Default: false.
    /// </summary>
    public bool DoNotValidateWorkflow { get; set; }

    /// <summary>
    /// True to check referrers of this content.
    /// </summary>
    public bool CheckReferrers { get; set; }

    /// <summary>
    /// True to turn off costly validation: Unique checks, asset checks and reference checks. Default: true.
    /// </summary>
    public bool OptimizeValidation { get; set; } = true;
}
