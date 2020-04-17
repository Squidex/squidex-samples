// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation
{
    public interface ISession
    {
        string App { get; }

        IAppsClient Apps { get; }

        IBackupsClient Backups { get; }

        ISchemasClient Schemas { get; }

        IContentsClient<DummyEntity, DummyData> Contents(string schema);
    }
}
