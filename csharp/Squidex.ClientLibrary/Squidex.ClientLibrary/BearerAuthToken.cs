// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.Assets.Internal;

namespace Squidex.ClientLibrary;

/// <summary>
/// A bearer auth token.
/// </summary>
public sealed class BearerAuthToken : AuthToken
{
    /// <summary>
    /// The bearer token.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BearerAuthToken" /> class with all properties.
    /// </summary>
    /// <param name="value">The token.</param>
    public BearerAuthToken(string value)
    {
        Guard.NotNullOrEmpty(value, nameof(value));

        Value = value;
    }

    /// <inheritdoc />
    public override (string Name, string Value) SerializeAsHeader()
    {
        return ("Authorization", $"Bearer {Value}");
    }

    /// <inheritdoc />
    public override (string Name, string Value) SerializeAsQuery()
    {
        return ("access_token", Value);
    }
}
