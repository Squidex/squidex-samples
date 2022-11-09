// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary;

/// <summary>
/// Base class for options.
/// </summary>
public abstract class OptionsBase
{
    private bool isFrozen;

    /// <summary>
    /// Indicates whether the options are frozen.
    /// </summary>
    public bool IsFrozen => isFrozen;

    /// <summary>
    /// Sets a value if not frozen.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="target">The target field.</param>
    /// <param name="value">The value.</param>
    protected void Set<T>(ref T target, T value)
    {
        ThrowIfFrozen();
        target = value;
    }

    /// <summary>
    /// Frezes the options.
    /// </summary>
    protected void Freeze()
    {
        isFrozen = true;
    }

    private void ThrowIfFrozen()
    {
        if (isFrozen)
        {
            throw new InvalidOperationException("Options are frozen and cannot be changed.");
        }
    }
}
