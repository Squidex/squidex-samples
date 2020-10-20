// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Utils;

// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

namespace Squidex.ClientLibrary
{
    [Obsolete("Use the new content client instead.")]
    public sealed class SquidexClient<TEntity, TData> : SquidexClientBase where TEntity : SquidexEntityBase<TData> where TData : class, new()
    {
        public string SchemaName { get; }

        public SquidexClient(SquidexOptions options, string schemaName, HttpClient httpClient)
            : base(options, httpClient)
        {
            Guard.NotNullOrEmpty(schemaName, nameof(schemaName));

            SchemaName = schemaName;
        }

        public async Task<TResponse> GraphQlAsync<TResponse>(object request, QueryContext context = null, CancellationToken ct = default)
        {
            Guard.NotNull(request, nameof(request));

            var response = await RequestJsonAsync<GraphQlResponse<TResponse>>(HttpMethod.Post, BuildAppUrl("graphql"), request.ToContent(), context, ct);

            if (response.Errors?.Length > 0)
            {
                throw new SquidexGraphQlException(response.Errors);
            }

            return response.Data;
        }

        public Task<SquidexEntities<TEntity, TData>> GetAsync(HashSet<string> ids, QueryContext context = null, CancellationToken ct = default)
        {
            Guard.NotNull(ids, nameof(ids));
            Guard.NotNullOrEmpty(ids, nameof(ids));

            var q = $"?ids={string.Join(",", ids)}";

            return RequestJsonAsync<SquidexEntities<TEntity, TData>>(HttpMethod.Get, BuildAppUrl(q), null, context, ct);
        }

        public Task<SquidexEntities<TEntity, TData>> GetAsync(ContentQuery query = null, QueryContext context = null, CancellationToken ct = default)
        {
            var q = query?.ToQuery(true) ?? string.Empty;

            return RequestJsonAsync<SquidexEntities<TEntity, TData>>(HttpMethod.Get, BuildSchemaUrl(q), null, context, ct);
        }

        public Task<TEntity> GetAsync(string id, QueryContext context = null, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestJsonAsync<TEntity>(HttpMethod.Get, BuildSchemaUrl($"{id}/"), null, context, ct);
        }

        public Task<TEntity> CreateAsync(TData data, bool publish = false, CancellationToken ct = default)
        {
            Guard.NotNull(data, nameof(data));

            return RequestJsonAsync<TEntity>(HttpMethod.Post, BuildSchemaUrl($"?publish={publish}"), data.ToContent(), ct: ct);
        }

        public Task<TEntity> UpdateAsync(string id, TData data, bool asDraft = false, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));
            Guard.NotNull(data, nameof(data));

            return RequestJsonAsync<TEntity>(HttpMethod.Put, BuildSchemaUrl($"{id}/?asDraft={asDraft}"), data.ToContent(), ct: ct);
        }

        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct = default)
        {
            Guard.NotNull(entity, nameof(entity));

            return UpdateAsync(entity.Id, entity.Data, false, ct);
        }

        public Task<TEntity> PatchAsync<TPatch>(string id, TPatch patch, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));
            Guard.NotNull(patch, nameof(patch));

            return RequestJsonAsync<TEntity>(HttpMethodEx.Patch, BuildSchemaUrl($"{id}/"), patch.ToContent(), ct: ct);
        }

        public Task<TEntity> PatchAsync<TPatch>(TEntity entity, TPatch patch, CancellationToken ct = default)
        {
            Guard.NotNull(entity, nameof(entity));

            return PatchAsync(entity.Id, patch, ct);
        }

        public Task<TEntity> ChangeStatusAsync(string id, string status, CancellationToken ct = default)
        {
            Guard.NotNull(id, nameof(id));
            Guard.NotNull(status, nameof(status));

            return RequestJsonAsync<TEntity>(HttpMethod.Put, BuildSchemaUrl($"{id}/status"), new { status }.ToContent(), ct: ct);
        }

        public Task<TEntity> ChangeStatusAsync(TEntity entity, string status, CancellationToken ct = default)
        {
            Guard.NotNull(entity, nameof(entity));

            return ChangeStatusAsync(entity.Id, status, ct);
        }

        [Obsolete("Use ChangeStatus instead.")]
        public Task PublishAsync(string id)
        {
            return ChangeStatusAsync(id, Status.Published);
        }

        [Obsolete("Use ChangeStatus instead.")]
        public Task PublishAsync(TEntity entity)
        {
            return ChangeStatusAsync(entity, Status.Published);
        }

        [Obsolete("Use ChangeStatus instead.")]
        public Task UnpublishAsync(string id)
        {
            return ChangeStatusAsync(id, Status.Draft);
        }

        [Obsolete("Use ChangeStatus instead.")]
        public Task UnpublishAsync(TEntity entity)
        {
            return ChangeStatusAsync(entity, Status.Draft);
        }

        [Obsolete("Use ChangeStatus instead.")]
        public Task ArchiveAsync(string id)
        {
            return ChangeStatusAsync(id, Status.Archived);
        }

        [Obsolete("Use ChangeStatus instead.")]
        public Task ArchiveAsync(TEntity entity)
        {
            return ChangeStatusAsync(entity, Status.Archived);
        }

        [Obsolete("Use ChangeStatus instead.")]
        public Task RestoreAsync(string id)
        {
            return ChangeStatusAsync(id, Status.Draft);
        }

        [Obsolete("Use ChangeStatus instead.")]
        public Task RestoreAsync(TEntity entity)
        {
            return ChangeStatusAsync(entity, Status.Draft);
        }

        public Task DeleteAsync(string id, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Delete, BuildSchemaUrl($"{id}/"), ct: ct);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
        {
            Guard.NotNull(entity, nameof(entity));

            await DeleteAsync(entity.Id, ct);
        }

        private string BuildSchemaUrl(string path = "")
        {
            return $"content/{ApplicationName}/{SchemaName}/{path}";
        }

        private string BuildAppUrl(string path = "")
        {
            return $"content/{ApplicationName}/{path}";
        }
    }
}
