﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

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
        /// Generates an absolute URL.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <returns>
        /// The absolute URL.
        /// </returns>
        string? GenerateUrl(string? relativeUrl);

        /// <summary>
        /// Generates an absolute URL for the asset CDN.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <exception cref="InvalidOperationException">Asset CDN not configured.</exception>
        /// <returns>
        /// The absolute URL.
        /// </returns>
        string? GenerateAssetCDNUrl(string relativeUrl);

        /// <summary>
        /// Generates an absolute URL for the content CDN.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <exception cref="InvalidOperationException">Content CDN not configured.</exception>
        /// <returns>
        /// The absolute URL.
        /// </returns>
        string? GenerateContentCDNUrl(string relativeUrl);

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
        /// Creates a client instance to query diagnostics data.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IDiagnosticsClient CreateDiagnosticsClient();

        /// <summary>
        /// Creates a client instance to query and manage event consumers.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IEventConsumersClient CreateEventConsumersClient();

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
        /// Creates a client instance to make searches across content and records.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        ISearchClient CreateSearchClient();

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
        /// Creates a client instance to query and manage teams.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        ITeamsClient CreateTeamsClient();

        /// <summary>
        /// Creates a client instance to query templates.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        ITemplatesClient CreateTemplatesClient();

        /// <summary>
        /// Creates a client instance to translate content.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        ITranslationsClient CreateTranslationsClient();

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
        /// Creates a client instance to query and manage users as administrator.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IUserManagementClient CreateUserManagementClient();

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
        /// Creates a client instance to query and manage untyped rules.
        /// </summary>
        /// <param name="appName">The name of the app. Cannot be null.</param>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="appName"/> is null.</exception>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IExtendableRulesClient CreateExtendableRulesClient(string appName);

        /// <summary>
        /// Creates a client instance to query and manage contents for a schema.
        /// </summary>
        /// <typeparam name="TEntity">The type for the content entity.</typeparam>
        /// <typeparam name="TData">The type that represents the data structure.</typeparam>
        /// <param name="schemaName">The name of the schema. Cannot be null.</param>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="schemaName"/> is null.</exception>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IContentsClient<TEntity, TData> CreateContentsClient<TEntity, TData>(string schemaName) where TEntity : Content<TData> where TData : class, new();

        /// <summary>
        /// Creates a client instance to query and manage contents for an app and schema.
        /// </summary>
        /// <typeparam name="TEntity">The type for the content entity.</typeparam>
        /// <typeparam name="TData">The type that represents the data structure.</typeparam>
        /// <param name="appName">The name of the app. Cannot be null.</param>
        /// <param name="schemaName">The name of the schema. Cannot be null.</param>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="appName"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="schemaName"/> is null.</exception>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IContentsClient<TEntity, TData> CreateContentsClient<TEntity, TData>(string appName, string schemaName) where TEntity : Content<TData> where TData : class, new();

        /// <summary>
        /// Creates a client instance to query and manage contents for a schema with dynamic data shape.
        /// </summary>
        /// <param name="schemaName">The name of the schema. Cannot be null.</param>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="schemaName"/> is null.</exception>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IContentsClient<DynamicContent, DynamicData> CreateDynamicContentsClient(string schemaName);

        /// <summary>
        /// Creates a client instance to query and manage contents for an app and schema with dynamic data shape.
        /// </summary>
        /// <param name="appName">The name of the app. Cannot be null.</param>
        /// <param name="schemaName">The name of the schema. Cannot be null.</param>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="appName"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="schemaName"/> is null.</exception>
        /// <remarks>
        /// Do not create new clients frequently.
        /// </remarks>
        IContentsClient<DynamicContent, DynamicData> CreateDynamicContentsClient(string appName, string schemaName);

        /// <summary>
        /// Creates a <see cref="HttpClient"/> to make all kind of authorized requests.
        /// </summary>
        /// <returns>
        /// The created client.
        /// </returns>
        /// <remarks>
        /// Remember to return the client after each request.
        /// </remarks>
        HttpClient CreateHttpClient();

        /// <summary>
        /// Returns the http client.
        /// </summary>
        /// <param name="httpClient">The HTTP client to return.</param>
        void ReturnHttpClient(HttpClient httpClient);
    }
}
