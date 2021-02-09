// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// A client to query and manage content items.
    /// </summary>
    /// <typeparam name="TEntity">The type for the content entity.</typeparam>
    /// <typeparam name="TData">The type that represents the data structure.</typeparam>
    public interface IContentsClient<TEntity, TData> where TEntity : Content<TData> where TData : class, new()
    {
        /// <summary>
        /// Gets the name of the schema for which this client has been created.
        /// </summary>
        /// <value>
        /// The name of the schema for which this client has been created.
        /// </value>
        string SchemaName { get; }

        /// <summary>
        /// Creates a new content item from a data object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="publish">if set to <c>true</c> the content will be published.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The created content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        Task<TEntity> CreateAsync(TData data, bool publish = false, CancellationToken ct = default);

        /// <summary>
        /// Creates a new content item from a data object with a custom ID.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="id">The optional custom ID for the content item.</param>
        /// <param name="publish">if set to <c>true</c> the content will be published.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The created content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        Task<TEntity> CreateAsync(TData data, string id, bool publish = false, CancellationToken ct = default);

        /// <summary>
        /// Creates a new draft version for the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item. Cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> CreateDraftAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Creates a new draft version for the specified content item.
        /// </summary>
        /// <param name="entity">The content item.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task<TEntity> CreateDraftAsync(TEntity entity, CancellationToken ct = default);

        /// <summary>
        /// Change the status of the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item to change. Cannot be null or empty.</param>
        /// <param name="status">The new status of the content item.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="status"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        /// <exception cref="ArgumentException"><paramref name="status"/> is empty.</exception>
        Task<TEntity> ChangeStatusAsync(string id, string status, CancellationToken ct = default);

        /// <summary>
        /// Change the status of the specified content item.
        /// </summary>
        /// <param name="entity">The content item to change.</param>
        /// <param name="status">The new status of the content item.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="status"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="status"/> is empty.</exception>
        Task<TEntity> ChangeStatusAsync(TEntity entity, string status, CancellationToken ct = default);

        /// <summary>
        /// Patch the data of the content item with the specified ID.
        /// </summary>
        /// <typeparam name="TPatch">The partial data to update.</typeparam>
        /// <param name="id">The ID of the content item to patch. Cannot be null or empty.</param>
        /// <param name="patch">The partial data.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="patch"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> PatchAsync<TPatch>(string id, TPatch patch, CancellationToken ct = default);

        /// <summary>
        /// Patch the data of the specified content item.
        /// </summary>
        /// <typeparam name="TPatch">The partial data to update.</typeparam>
        /// <param name="entity">The content item to patch.</param>
        /// <param name="patch">The partial data.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="patch"/> is null.</exception>
        Task<TEntity> PatchAsync<TPatch>(TEntity entity, TPatch patch, CancellationToken ct = default);

        /// <summary>
        /// Updates or creates the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item to update or create. Cannot be null or empty.</param>
        /// <param name="data">The full data.</param>
        /// <param name="publish">if set to <c>true</c> the content will be published.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated or created content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> UpsertAsync(string id, TData data, bool publish = false, CancellationToken ct = default);

        /// <summary>
        /// Updates or creates the specified content item.
        /// </summary>
        /// <param name="entity">The content item to update or create.</param>
        /// <param name="publish">if set to <c>true</c> the content will be published.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated or created content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task<TEntity> UpsertAsync(TEntity entity, bool publish = false, CancellationToken ct = default);

        /// <summary>
        /// Updates the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item to update. Cannot be null or empty.</param>
        /// <param name="data">The full data.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> UpdateAsync(string id, TData data, CancellationToken ct = default);

        /// <summary>
        /// Updates the specified content item.
        /// </summary>
        /// <param name="entity">The content item to update.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct = default);

        /// <summary>
        /// Deletes the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item to delete. Cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task DeleteAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Deletes the specified content item.
        /// </summary>
        /// <param name="entity">The content item to delete.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task DeleteAsync(TEntity entity, CancellationToken ct = default);

        /// <summary>
        /// Deletes the draft version of the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item to update. Cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> DeleteDraftAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Deletes the draft version of the specified content item.
        /// </summary>
        /// <param name="entity">The content item to update.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task<TEntity> DeleteDraftAsync(TEntity entity, CancellationToken ct = default);

        /// <summary>
        /// Executes a bulk update.
        /// </summary>
        /// <param name="update">The bulk update.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The result of the bulk update.
        /// </returns>
        Task<List<BulkResult>> BulkUpdateAsync(BulkUpdate update, CancellationToken ct = default);

        /// <summary>
        /// Gets all content items in batches.
        /// </summary>
        /// <param name="batchSize">Size of each batch.</param>
        /// <param name="callback">The callbac that is invoked for each content item..</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
        Task GetAllAsync(int batchSize, Func<TEntity, Task> callback, CancellationToken ct = default);

        /// <summary>
        /// Gets a content item by ID.
        /// </summary>
        /// <param name="id">The ID of the content item. Cannot be null or empty.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when querying content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The content item or null if not found.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> GetAsync(string id, QueryContext context = null, CancellationToken ct = default);

        /// <summary>
        /// Query content items by an optional query.
        /// </summary>
        /// <param name="query">The optional query.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when querying content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The matching content items.
        /// </returns>
        Task<ContentsResult<TEntity, TData>> GetAsync(ContentQuery query = null, QueryContext context = null, CancellationToken ct = default);

        /// <summary>
        /// Query all contents items that reference the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item. Cannot be null or empty.</param>
        /// <param name="query">The optional query.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when querying content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The matching content items.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<ContentsResult<TEntity, TData>> GetReferencingAsync(string id, ContentQuery query = null, QueryContext context = null, CancellationToken ct = default);

        /// <summary>
        /// Query all contents items that reference the specified content item.
        /// </summary>
        /// <param name="entity">The content item. Cannot be null.</param>
        /// <param name="query">The optional query.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when querying content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The matching content items.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task<ContentsResult<TEntity, TData>> GetReferencingAsync(TEntity entity, ContentQuery query = null, QueryContext context = null, CancellationToken ct = default);

        /// <summary>
        /// Query all references of the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item. Cannot be null or empty.</param>
        /// <param name="query">The optional query.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when querying content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The matching content items.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<ContentsResult<TEntity, TData>> GetReferencesAsync(string id, ContentQuery query = null, QueryContext context = null, CancellationToken ct = default);

        /// <summary>
        /// Query all references of the specified content item.
        /// </summary>
        /// <param name="entity">The content item. Cannot be null.</param>
        /// <param name="query">The optional query.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when querying content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The matching content items.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task<ContentsResult<TEntity, TData>> GetReferencesAsync(TEntity entity, ContentQuery query = null, QueryContext context = null, CancellationToken ct = default);

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
        Task<ContentsResult<TEntity, TData>> GetAsync(HashSet<string> ids, QueryContext context = null, CancellationToken ct = default);

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
        Task<TResponse> GraphQlAsync<TResponse>(object request, QueryContext context = null, CancellationToken ct = default);

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
        Task<IEnumerable<GraphQlResponse<TResponse>>> GraphQlAsync<TResponse>(IEnumerable<object> requests, QueryContext context = null, CancellationToken ct = default);

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
        Task<TResponse> GraphQlGetAsync<TResponse>(object request, QueryContext context = null, CancellationToken ct = default);
    }
}