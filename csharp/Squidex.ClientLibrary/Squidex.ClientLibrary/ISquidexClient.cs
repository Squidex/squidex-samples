// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Management;

namespace Squidex.ClientLibrary;

/// <summary>
/// Provides access to all endpoints through individual clients and handles authentication.
/// </summary>
public interface ISquidexClient
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
    /// Gets a client instance to query and manage app configuration.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IAppsClient Apps { get; }

    /// <summary>
    /// Gets a client instance to query and manage assets.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IAssetsClient Assets { get; }

    /// <summary>
    /// Gets a client instance to query and manage backups.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IBackupsClient Backups { get; }

    /// <summary>
    /// Gets a client instance to query and manage comments.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    ICommentsClient Comments { get; }

    /// <summary>
    /// Gets a client instance to query diagnostics data.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IDiagnosticsClient Diagnostics { get; }

    /// <summary>
    /// Gets a client instance to query and manage event consumers.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IEventConsumersClient EventConsumers { get; }

    /// <summary>
    /// Gets a client instance to query and manage histories.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IHistoryClient History { get; }

    /// <summary>
    /// Gets a client instance to query all supported languages.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    /// <remarks>
    /// Do not create new clients frequently.
    /// </remarks>
    ILanguagesClient Languages { get; }

    /// <summary>
    /// Gets a client instance to ping the server for monitoring.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IPingClient Ping { get; }

    /// <summary>
    /// Gets a client instance to query and manage plans and subscriptions.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IPlansClient Plans { get; }

    /// <summary>
    /// Gets a client instance to query and manage rules.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IRulesClient Rules { get; }

    /// <summary>
    /// Gets a client instance to query and manage schemas.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    ISchemasClient Schemas { get; }

    /// <summary>
    /// Gets a client instance to make searches across content and records.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    ISearchClient Search { get; }

    /// <summary>
    /// Gets a client instance to query statistics.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IStatisticsClient Statistics { get; }

    /// <summary>
    /// Gets a client instance to query and manage teams.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    ITeamsClient Teams { get; }

    /// <summary>
    /// Gets a client instance to query templates.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    ITemplatesClient Templates { get; }

    /// <summary>
    /// Gets a client instance to translate content.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    ITranslationsClient Translations { get; }

    /// <summary>
    /// Gets a client instance to query user information.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IUsersClient Users { get; }

    /// <summary>
    /// Gets a client instance to query and manage users as administrator.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IUserManagementClient UserManagement { get; }

    /// <summary>
    /// Gets a client instance to query and manage untyped rules.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IExtendableRulesClient ExtendableRules { get; }

    /// <summary>
    /// Gets a client instance to query and manage contents across all schemas with dynamic data shape.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    IContentsSharedClient<DynamicContent, DynamicData> SharedDynamicContents { get; }

    /// <summary>
    /// Gets a client instance to query and manage contents for a schema.
    /// </summary>
    /// <typeparam name="TEntity">The type for the content entity.</typeparam>
    /// <typeparam name="TData">The type that represents the data structure.</typeparam>
    /// <param name="schemaName">The name of the schema. Cannot be null or empty.</param>
    /// <returns>
    /// The returned client.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="schemaName"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="schemaName"/> is empty.</exception>
    IContentsClient<TEntity, TData> Contents<TEntity, TData>(string schemaName) where TEntity : Content<TData> where TData : class, new();

    /// <summary>
    /// Gets a client instance to query and manage contents for a schema with dynamic data shape.
    /// </summary>
    /// <param name="schemaName">The name of the schema. Cannot be null or empty.</param>
    /// <returns>
    /// The returned client.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="schemaName"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="schemaName"/> is empty.</exception>
    IContentsClient<DynamicContent, DynamicData> DynamicContents(string schemaName);

    /// <summary>
    /// Gets a client instance to query and manage contents across all schemas.
    /// </summary>
    /// <typeparam name="TEntity">The type for the content entity.</typeparam>
    /// <typeparam name="TData">The type that represents the data structure.</typeparam>
    /// <returns>
    /// The returned client.
    /// </returns>
    IContentsSharedClient<TEntity, TData> SharedContents<TEntity, TData>() where TEntity : Content<TData> where TData : class, new();

    /// <summary>
    /// Creates a <see cref="HttpClient"/> to make all kind of authorized requests.
    /// </summary>
    /// <returns>
    /// The returned client.
    /// </returns>
    HttpClient CreateHttpClient();
}
