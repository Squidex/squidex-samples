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
    /// <summary>
    /// Provides access to all endpoints through individual clients and handles authentication.
    /// </summary>
    public interface ISquidexClientManager
    {
        /// <summary>
        /// Gets the options that are used to initialize the client manager.
        /// </summary>
        /// <value>
        /// The options that are used to initialize the client manager.
        /// </value>
        /// <remarks>
        /// This object is frozen and cannot be changed later.
        /// </remarks>
        SquidexOptions Options { get; }

        /// <summary>
        /// Gets the name of the App.
        /// </summary>
        /// <value>
        /// The name of the App.
        /// </value>
        string App { get; }

        /// <summary>
        /// Generates the URL to the image with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the asset.</param>
        /// <returns>
        /// THe URL to the image.
        /// </returns>
        string? GenerateImageUrl(string? id);

        /// <summary>
        /// Generates the URL to the image with the first specified ID.
        /// </summary>
        /// <param name="id">The ID of the asset.</param>
        /// <returns>
        /// THe URL to the image.
        /// </returns>
        string? GenerateImageUrl(IEnumerable<string>? id);

        /// <summary>
        /// Creates a client instance to query and manage app configuration.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IAppsClient CreateAppsClient();

        /// <summary>
        /// Creates a client instance to query and manage assets.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IAssetsClient CreateAssetsClient();

        /// <summary>
        /// Creates a client instance to query and manage backups.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IBackupsClient CreateBackupsClient();

        /// <summary>
        /// Creates a client instance to query and manage comments.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        ICommentsClient CreateCommentsClient();

        /// <summary>
        /// Creates a client instance to query and manage histories.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IHistoryClient CreateHistoryClient();

        /// <summary>
        /// Creates a client instance to query all supported languages.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        ILanguagesClient CreateLanguagesClient();

        /// <summary>
        /// Creates a client instance to ping the server for monitoring.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IPingClient CreatePingClient();

        /// <summary>
        /// Creates a client instance to query and manage plans and subscriptions.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IPlansClient CreatePlansClient();

        /// <summary>
        /// Creates a client instance to query and manage rules.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IRulesClient CreateRulesClient();

        /// <summary>
        /// Creates a client instance to query and manage schemas.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        ISchemasClient CreateSchemasClient();

        /// <summary>
        /// Creates a client instance to query statistics.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IStatisticsClient CreateStatisticsClient();

        /// <summary>
        /// Creates a client instance to query user information.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IUsersClient CreateUsersClient();

        /// <summary>
        /// Creates a client instance to query and manage untyped rules.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IExtendableRulesClient CreateExtendableRulesClient();

        /// <summary>
        /// Creates a client instance to query and manage contents for a schema.
        /// </summary>
        /// <typeparam name="TEntity">The type for the content entity.</typeparam>
        /// <typeparam name="TData">The type that represents the data structure.</typeparam>
        /// <param name="schemaName">The name of the schema. Cannot be null.</param>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="schemaName"/> is null.</exception>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IContentsClient<TEntity, TData> CreateContentsClient<TEntity, TData>(string schemaName) where TEntity : Content<TData> where TData : class, new();

        /// <summary>
        /// Creates a client instance to query and manage contents for a schema with dynamic data shape.
        /// </summary>
        /// <param name="schemaName">The name of the schema. Cannot be null.</param>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="schemaName"/> is null.</exception>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IContentsClient<DynamicContent, DynamicData> CreateDynamicContentsClient(string schemaName);

        /// <summary>
        /// Creates a <see cref="HttpClient"/> to make all kind of authorized requests.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        HttpClient CreateHttpClient();
    }
}
