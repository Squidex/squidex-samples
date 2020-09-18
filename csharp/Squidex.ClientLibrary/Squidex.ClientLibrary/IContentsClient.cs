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
    public interface IContentsClient<TEntity, TData> where TEntity : Content<TData> where TData : class, new()
    {
        string SchemaName { get; }

        Task<TEntity> CreateAsync(TData data, bool publish = false, CancellationToken ct = default);

        Task<TEntity> CreateAsync(TData data, string id, bool publish = false, CancellationToken ct = default);

        Task<TEntity> CreateDraftAsync(string id, CancellationToken ct = default);

        Task<TEntity> CreateDraftAsync(TEntity entity, CancellationToken ct = default);

        Task<TEntity> ChangeStatusAsync(string id, string status, CancellationToken ct = default);

        Task<TEntity> ChangeStatusAsync(TEntity entity, string status, CancellationToken ct = default);

        Task<TEntity> PatchAsync<TPatch>(string id, TPatch patch, CancellationToken ct = default);

        Task<TEntity> PatchAsync<TPatch>(TEntity entity, TPatch patch, CancellationToken ct = default);

        Task<TEntity> UpdateAsync(string id, TData data, bool asDraft = false, CancellationToken ct = default);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct = default);

        Task DeleteAsync(string id, CancellationToken ct = default);

        Task DeleteAsync(TEntity entity, CancellationToken ct = default);

        Task<TEntity> DeleteDraftAsync(string id, CancellationToken ct = default);

        Task<TEntity> DeleteDraftAsync(TEntity entity, CancellationToken ct = default);

        Task<List<BulkResult>> BulkUpdateAsync(BulkUpdate update, CancellationToken ct = default);

        Task GetAllAsync(int batchSize, Func<TEntity, Task> callback, CancellationToken ct = default);

        Task<TEntity> GetAsync(string id, QueryContext context = null, CancellationToken ct = default);

        Task<ContentsResult<TEntity, TData>> GetAsync(ContentQuery query = null, QueryContext context = null, CancellationToken ct = default);

        Task<ContentsResult<TEntity, TData>> GetAsync(HashSet<string> ids, QueryContext context = null, CancellationToken ct = default);

        Task<TResponse> GraphQlAsync<TResponse>(object request, QueryContext context = null, CancellationToken ct = default);

        Task<TResponse> GraphQlGetAsync<TResponse>(object request, QueryContext context = null, CancellationToken ct = default);
    }
}