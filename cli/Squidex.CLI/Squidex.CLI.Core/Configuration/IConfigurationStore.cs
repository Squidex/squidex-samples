// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Configuration;

public interface IConfigurationStore
{
    DirectoryInfo WorkingDirectory { get; }

    (T? Value, DateTimeOffset Saved) Get<T>(string key) where T : class;

    void Set<T>(string key, T value) where T : class;
}
