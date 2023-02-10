// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.FileSystem;

namespace Squidex.CLI.Commands.Implementation.Sync;

public interface ISynchronizer
{
    int Order => 0;

    string Name { get; }

    string Description { get; }

    Task CleanupAsync(IFileSystem fs);

    Task DescribeAsync(ISyncService sync, MarkdownWriter writer);

    Task ExportAsync(ISyncService sync, SyncOptions options, ISession session);

    Task ImportAsync(ISyncService sync, SyncOptions options, ISession session);

    Task GenerateSchemaAsync(ISyncService sync);
}
