// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation
{
    public interface ISession
    {
        string App { get; }

        string ClientId { get; }

        string ClientSecret { get; }

        DirectoryInfo WorkingDirectory { get; }

        IAppsClient Apps { get; }

        IAssetsClient Assets { get; }

        IBackupsClient Backups { get; }

        ISchemasClient Schemas { get; }

        IExtendableRulesClient Rules { get; }

        IContentsClient<TEntity, TData> Contents<TEntity, TData>(string schemaName) where TEntity : Content<TData> where TData : class, new();

        IContentsClient<DynamicContent, DynamicData> Contents(string schemaName);
    }
}
