// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary;

/// <summary>
/// The API key for authentication.
/// </summary>
public abstract class AuthToken
{
    /// <summary>
    /// Serializes the header value.
    /// </summary>
    /// <returns>The header value.</returns>
    public abstract (string Name, string Value) SerializeAsHeader();

    /// <summary>
    /// Serializes the query string value.
    /// </summary>
    /// <returns>The query string value.</returns>
    public abstract (string Name, string Value) SerializeAsQuery();
}
