// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

namespace Squidex.ClientLibrary
{
    public sealed class SquidexClient<TEntity, TData> : SquidexClientBase
        where TEntity : SquidexEntityBase<TData>
        where TData : class, new()
    {
        public SquidexClient(Uri serviceUrl, string applicationName, string schemaName, IAuthenticator authenticator)
            : base(serviceUrl, applicationName, schemaName, authenticator)
        {
        }

        public async Task<SquidexEntities<TEntity, TData>> GetAsync(long? skip = null, long? top = null, string filter = null, string orderBy = null, string search = null, QueryContext context = null)
        {
            var queries = new List<string>();

            if (skip.HasValue)
            {
                queries.Add($"$skip={skip.Value}");
            }

            if (top.HasValue)
            {
                queries.Add($"$top={top.Value}");
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                queries.Add($"$orderby={orderBy}");
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                queries.Add($"$search={search}");
            }
            else if (!string.IsNullOrWhiteSpace(filter))
            {
                queries.Add($"$filter={filter}");
            }

            var query = string.Join("&", queries);

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = "?" + query;
            }

            var response = await RequestAsync(HttpMethod.Get, BuildContentUrl(query), context: context);

            return await response.Content.ReadAsJsonAsync<SquidexEntities<TEntity, TData>>();
        }

        public async Task<TEntity> GetAsync(string id, QueryContext context = null)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            var response = await RequestAsync(HttpMethod.Get, BuildContentUrl($"{id}/"), context: context);
            return await response.Content.ReadAsJsonAsync<TEntity>();
        }

        public async Task<TEntity> CreateAsync(TData data, bool publish = false)
        {
            Guard.NotNull(data, nameof(data));

            var response = await RequestAsync(HttpMethod.Post, BuildContentUrl($"?publish={publish}"), data.ToContent());

            return await response.Content.ReadAsJsonAsync<TEntity>();
        }

        public Task UpdateAsync(string id, TData data, bool asDraft = false)
        {
            Guard.NotNull(data, nameof(data));
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Put, BuildContentUrl($"{id}/?asDraft={asDraft}"), data.ToContent());
        }

        public async Task UpdateAsync(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            await UpdateAsync(entity.Id, entity.Data);

            entity.MarkAsUpdated();
        }

        public Task PublishAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Put, BuildContentUrl($"{id}/publish/"));
        }

        public async Task PublishAsync(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            await PublishAsync(entity.Id);

            entity.MarkAsUpdated();
        }

        public Task UnpublishAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Put, BuildContentUrl($"{id}/unpublish/"));
        }

        public async Task UnpublishAsync(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            await UnpublishAsync(entity.Id);

            entity.MarkAsUpdated();
        }

        public Task ArchiveAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Put, BuildContentUrl($"{id}/archive/"));
        }

        public async Task ArchiveAsync(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            await ArchiveAsync(entity.Id);

            entity.MarkAsUpdated();
        }

        public Task RestoreAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Put, BuildContentUrl($"{id}/restore/"));
        }

        public async Task RestoreAsync(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            await RestoreAsync(entity.Id);

            entity.MarkAsUpdated();
        }

        public Task DeleteAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Delete, BuildContentUrl($"{id}/"));
        }

        public async Task DeleteAsync(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            await DeleteAsync(entity.Id);

            entity.MarkAsUpdated();
        }

        private string BuildContentUrl(string path = "")
        {
            return $"api/content/{ApplicationName}/{SchemaName}/{path}";
        }
    }
}
