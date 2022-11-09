// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Squidex.ClientLibrary;

/// <summary>
/// A value that can be null.
/// </summary>
/// <typeparam name="T">The actual value.</typeparam>
public record struct JsonNull<T>(T Value)
{
    /// <summary>
    /// Operator to compare the value to the wrapper.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator JsonNull<T>(T value)
    {
        return new JsonNull<T>(value);
    }

    /// <summary>
    /// Operator to compare the wrapper to the actual value.
    /// </summary>
    /// <param name="wrapper">The wrapper to convert.</param>
    public static implicit operator T(JsonNull<T> wrapper)
    {
        return wrapper.Value;
    }

    /// <inheritdoc/>
    public override readonly string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }
}
