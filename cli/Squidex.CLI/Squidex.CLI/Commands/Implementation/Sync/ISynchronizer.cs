// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using Squidex.CLI.Commands.Implementation.FileSystem;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public interface ISynchronizer
    {
        int Order => 0;

        string Name { get; }

        Task CleanupAsync(IFileSystem fs);

        Task ExportAsync(IFileSystem fs, JsonHelper jsonHelper, SyncOptions options, ISession session);

        Task ImportAsync(IFileSystem fs, JsonHelper jsonHelper, SyncOptions options, ISession session);

        Task GenerateSchemaAsync(IFileSystem directoryInfo, JsonHelper jsonHelper);
    }
}
