// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

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
        /// Gets the name of the app for which this client has been created.
        /// </summary>
        /// <value>
        /// The name of the app for which this client has been created.
        /// </value>
        string AppName { get; }

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
        /// <param name="options">The additional options.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The created content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        Task<TEntity> CreateAsync(TData data, ContentCreateOptions options = default, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Creates a new draft version for the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item. Cannot be null or empty.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> CreateDraftAsync(string id, ContentCreateDraftOptions options = default, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Creates a new draft version for the specified content item.
        /// </summary>
        /// <param name="entity">The content item.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task<TEntity> CreateDraftAsync(TEntity entity, ContentCreateDraftOptions options = default, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Change the status of the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item to change. Cannot be null or empty.</param>
        /// <param name="request">The status request.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> ChangeStatusAsync(string id, ChangeStatus request, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Change the status of the specified content item.
        /// </summary>
        /// <param name="entity">The content item to change.</param>
        /// <param name="request">The status request.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is null.</exception>
        Task<TEntity> ChangeStatusAsync(TEntity entity, ChangeStatus request, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Patch the data of the content item with the specified ID.
        /// </summary>
        /// <typeparam name="TPatch">The partial data to update.</typeparam>
        /// <param name="id">The ID of the content item to patch. Cannot be null or empty.</param>
        /// <param name="patch">The partial data.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="patch"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> PatchAsync<TPatch>(string id, TPatch patch, ContentPatchOptions options = default, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Patch the data of the specified content item.
        /// </summary>
        /// <typeparam name="TPatch">The partial data to update.</typeparam>
        /// <param name="entity">The content item to patch.</param>
        /// <param name="patch">The partial data.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="patch"/> is null.</exception>
        Task<TEntity> PatchAsync<TPatch>(TEntity entity, TPatch patch, ContentPatchOptions options = default, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Updates or creates the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item to update or create. Cannot be null or empty.</param>
        /// <param name="data">The full data.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated or created content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> UpsertAsync(string id, TData data, ContentUpsertOptions options = default, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Updates or creates the specified content item.
        /// </summary>
        /// <param name="entity">The content item to update or create.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated or created content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task<TEntity> UpsertAsync(TEntity entity, ContentUpsertOptions options = default, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Updates the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item to update. Cannot be null or empty.</param>
        /// <param name="data">The full data.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> UpdateAsync(string id, TData data, ContentUpdateOptions options = default, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Updates the specified content item.
        /// </summary>
        /// <param name="entity">The content item to update.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The updated content item.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task<TEntity> UpdateAsync(TEntity entity, ContentUpdateOptions options = default, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Deletes the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item to delete. Cannot be null or empty.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task DeleteAsync(string id, ContentDeleteOptions options = default,
             CancellationToken ct = default);

        /// <summary>
        /// Deletes the specified content item.
        /// </summary>
        /// <param name="entity">The content item to delete.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task DeleteAsync(TEntity entity, ContentDeleteOptions options = default,
             CancellationToken ct = default);

        /// <summary>
        /// Deletes the draft version of the content item with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the content item to update. Cannot be null or empty.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TEntity> DeleteDraftAsync(string id, ContentDeleteDraftOptions options = default, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Deletes the draft version of the specified content item.
        /// </summary>
        /// <param name="entity">The content item to update.</param>
        /// <param name="options">The additional options.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when retrieving content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
        Task<TEntity> DeleteDraftAsync(TEntity entity, ContentDeleteDraftOptions options = default, QueryContext? context = null,
             CancellationToken ct = default);

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
        /// Gets all content items in batches.
        /// </summary>
        /// <param name="callback">The callbac that is invoked for each content item.</param>
        /// <param name="batchSize">Size of each batch.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when querying content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
        Task GetAllAsync(Func<TEntity, Task> callback, int batchSize = 200, QueryContext? context = null,
             CancellationToken ct = default);

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
        Task<TEntity> GetAsync(string id, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Gets a content item by ID and version.
        /// </summary>
        /// <param name="id">The ID of the content item. Cannot be null or empty.</param>
        /// <param name="version">The version of the content.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when querying content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The content item or null if not found.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
        Task<TData> GetDataAsync(string id, int version, QueryContext? context = null,
             CancellationToken ct = default);

        /// <summary>
        /// Query content items by an optional query.
        /// </summary>
        /// <param name="query">The optional query.</param>
        /// <param name="context">The context object to add additonal headers to the request and change the behavior of the API when querying content items.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The matching content items.
        /// </returns>
        Task<ContentsResult<TEntity, TData>> GetAsync(ContentQuery? query = null, QueryContext? context = null,
             CancellationToken ct = default);

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
        Task<ContentsResult<TEntity, TData>> GetReferencingAsync(string id, ContentQuery? query = null, QueryContext? context = null,
             CancellationToken ct = default);

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
        Task<ContentsResult<TEntity, TData>> GetReferencingAsync(TEntity entity, ContentQuery? query = null, QueryContext? context = null,
             CancellationToken ct = default);

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
        Task<ContentsResult<TEntity, TData>> GetReferencesAsync(string id, ContentQuery? query = null, QueryContext? context = null,
             CancellationToken ct = default);

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
        Task<ContentsResult<TEntity, TData>> GetReferencesAsync(TEntity entity, ContentQuery? query = null, QueryContext? context = null,
             CancellationToken ct = default);
    }
}
