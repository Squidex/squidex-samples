// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.Assets.Internal;

namespace Squidex.ClientLibrary;

/// <summary>
/// A ApiKey auth token.
/// </summary>
public sealed class ApiKeyAuthToken : AuthToken
{
    /// <summary>
    /// The app name.
    /// </summary>
    public string AppName { get; }

    /// <summary>
    /// The bearer token.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthToken" /> class with all properties.
    /// </summary>
    /// <param name="appName">The app name.</param>
    /// <param name="value">The APIKey.</param>
    public ApiKeyAuthToken(string appName, string value)
    {
        Guard.NotNullOrEmpty(appName, nameof(appName));
        Guard.NotNullOrEmpty(value, nameof(value));

        AppName = appName;

        Value = value;
    }

    /// <inheritdoc />
    public override (string Name, string Value) SerializeAsQuery()
    {
        return ("api_key", Uri.EscapeDataString($"{AppName}:{Value}"));
    }

    /// <inheritdoc />
    public override (string Name, string Value) SerializeAsHeader()
    {
        return ("Authorization", $"ApiKey {AppName}:{Value}");
    }
}
