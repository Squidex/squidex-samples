// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Management;

namespace Squidex.ClientLibrary;

/// <summary>
/// Represents the result of one <see cref="BulkUpdateJob"/>.
/// </summary>
public sealed class BulkResult
{
    /// <summary>
    /// The id of the content that has been handled successfully or not.
    /// </summary>
    public string ContentId { get; set; }

    /// <summary>
    /// The index of the bulk job where the result belongs to. The order can change.
    /// </summary>
    public int JobIndex { get; set; }

    /// <summary>
    /// The error when the bulk job failed.
    /// </summary>
    public ErrorDto Error { get; set; }
}
