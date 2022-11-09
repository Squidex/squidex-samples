// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary;

/// <summary>
/// A client to query and manage content items across schemas.
/// </summary>
/// <typeparam name="TEntity">The type for the content entity.</typeparam>
/// <typeparam name="TData">The type that represents the data structure.</typeparam>
public interface IContentsSharedClient<TEntity, TData> where TEntity : Content<TData> where TData : class, new()
{
    /// <summary>
    /// Gets the name of the app for which this client has been created.
    /// </summary>
    /// <value>
    /// The name of the app for which this client has been created.
    /// </value>
    string AppName { get; }

    /// <summary>
    /// Executes a bulk update.
    /// </summary>
    /// <param name="update">The bulk update.</param>
    /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The result of the bulk update.
    /// </returns>
    Task<List<BulkResult>> BulkUpdateAsync(BulkUpdate update,
         CancellationToken ct = default);

    /// <summary>
    /// Gets content items by ids across all schemas.
    /// </summary>
    /// <param name="ids">The ids to query.</param>
    /// <param name="context">The context object to add additonal headers to the request and
    /// change the behavior of the API when querying content items.</param>
    /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The matching content items.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="ids"/> is null.</exception>
    /// <remarks>
    /// Even though this method is part of the content client that is created for a specific schemas, it can return content items
    /// across all schemas in your App.
    /// </remarks>
    Task<ContentsResult<TEntity, TData>> GetAsync(HashSet<string> ids, QueryContext? context = null,
         CancellationToken ct = default);

    /// <summary>
    /// Executes a GrapQL query as HTTP POST method.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The GraphQL request. Cannot be null.</param>
    /// <param name="context">The context object to add additonal headers to the request and
    /// change the behavior of the API when querying content items.</param>
    /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The result of the query.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is null.</exception>
    Task<TResponse> GraphQlAsync<TResponse>(object request, QueryContext? context = null,
         CancellationToken ct = default);

    /// <summary>
    /// Executes multiple GrapQL queries as HTTP POST method.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="requests">The GraphQL requests. Cannot be null.</param>
    /// <param name="context">The context object to add additonal headers to the request and
    /// change the behavior of the API when querying content items.</param>
    /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The result of the query.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="requests"/> is null.</exception>
    Task<IEnumerable<GraphQlResponse<TResponse>>> GraphQlAsync<TResponse>(IEnumerable<object> requests, QueryContext? context = null,
         CancellationToken ct = default);

    /// <summary>
    /// Executes a GrapQL query as HTTP GET method.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The GraphQL request. Cannot be null.</param>
    /// <param name="context">The context object to add additonal headers to the request and
    /// change the behavior of the API when querying content items.</param>
    /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The result of the query.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is null.</exception>
    Task<TResponse> GraphQlGetAsync<TResponse>(object request, QueryContext? context = null,
         CancellationToken ct = default);
}
