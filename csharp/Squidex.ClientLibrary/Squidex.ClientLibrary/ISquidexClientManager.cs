// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Net.Http;
using Squidex.ClientLibrary.Management;

namespace Squidex.ClientLibrary
{
    public interface ISquidexClientManager
    {
        SquidexOptions Options { get; }

        string GenerateImageUrl(string id);

        string GenerateImageUrl(IEnumerable<string> id);

        IAppsClient CreateAppsClient();

        IAssetsClient CreateAssetsClient();

        IBackupsClient CreateBackupsClient();

        ICommentsClient CreateCommentsClient();

        IHistoryClient CreateHistoryClient();

        ILanguagesClient CreateLanguagesClient();

        IPingClient CreatePingClient();

        IPlansClient CreatePlansClient();

        IRulesClient CreateRulesClient();

        ISchemasClient CreateSchemasClient();

        IStatisticsClient CreateStatisticsClient();

        IUsersClient CreateUsersClient();

        IExtendableRulesClient CreateExtendableRulesClient();

        IContentsClient<TEntity, TData> CreateContentsClient<TEntity, TData>(string schemaName) where TEntity : Content<TData> where TData : class, new();

        HttpClient CreateHttpClient();
    }
}