// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO;
using System.Threading.Tasks;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public interface ISynchronizer
    {
        int Order => 0;

        string Name { get; }

        Task ExportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session);

        Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session);

        Task GenerateSchemaAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper);
    }
}
