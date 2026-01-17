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
public sealed class AuthToken
{
    /// <summary>
    /// Gets the header name.
    /// </summary>
    public string HeaderName { get; }

    /// <summary>
    /// Gets the header value.
    /// </summary>
    public string HeaderValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthToken" /> class with all properties.
    /// </summary>
    /// <param name="headerName">The header name. Cannot be null or empty.</param>
    /// <param name="headerValue">The header value. Cannot be null or empty.</param>
    public AuthToken(string headerName, string headerValue)
    {
        Guard.NotNullOrEmpty(headerName, nameof(headerName));
        Guard.NotNullOrEmpty(headerValue, nameof(headerValue));

        HeaderName = headerName;
        HeaderValue = headerValue;
    }
}
