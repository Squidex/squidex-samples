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

        string ClientId { get; }

        IAppsClient Apps { get; }

        IAssetsClient Assets { get; }

        IBackupsClient Backups { get; }

        ISchemasClient Schemas { get; }

        IExtendableRulesClient Rules { get; }

        IContentsClient<DynamicContent, DynamicData> Contents(string schema);
    }
}
