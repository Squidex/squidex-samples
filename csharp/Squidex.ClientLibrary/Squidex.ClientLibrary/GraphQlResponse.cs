// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary;

/// <summary>
/// A graphql response.
/// </summary>
/// <typeparam name="TData">The type of the data.</typeparam>
public sealed class GraphQlResponse<TData>
{
    /// <summary>
    /// The data object. Can be null when no result is returned on an error occurred.
    /// </summary>
    public TData Data { get; set; }

    /// <summary>
    /// The errors. Can be null when valid.
    /// </summary>
    public GraphQlError[] Errors { get; set; }
}
