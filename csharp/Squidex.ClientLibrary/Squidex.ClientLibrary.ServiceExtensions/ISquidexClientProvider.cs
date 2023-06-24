// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.ServiceExtensions;

/// <summary>
/// Provides a client.
/// </summary>
public interface ISquidexClientProvider
{
    /// <summary>
    /// Gets the default client.
    /// </summary>
    /// <returns>
    /// The default client.
    /// </returns>
    ISquidexClient Get();

    /// <summary>
    /// Gets the client with the given name.
    /// </summary>
    /// <param name="name">The client name.</param>
    /// <returns>
    /// The client with the given name.
    /// </returns>
    ISquidexClient Get(string name);
}
