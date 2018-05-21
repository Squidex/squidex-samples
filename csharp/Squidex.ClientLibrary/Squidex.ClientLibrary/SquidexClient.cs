// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

namespace Squidex.ClientLibrary
{
    public sealed class SquidexClient<TEntity, TData> where TData : class, new() where TEntity : SquidexEntityBase<TData>
    {
        private readonly string applicationName;
        private readonly Uri serviceUrl;
        private readonly string schemaName;
        private readonly IAuthenticator authenticator;

        public SquidexClient(Uri serviceUrl, string applicationName, string schemaName, IAuthenticator authenticator)
        {
            Guard.NotNull(serviceUrl, nameof(serviceUrl));
            Guard.NotNull(authenticator, nameof(authenticator));
            Guard.NotNullOrEmpty(schemaName, nameof(schemaName));
            Guard.NotNullOrEmpty(applicationName, nameof(applicationName));

            this.serviceUrl = serviceUrl;
            this.schemaName = schemaName;
            this.authenticator = authenticator;
            this.applicationName = applicationName;
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

            var response = await RequestAsync(HttpMethod.Get, query, context: context);

            return await response.Content.ReadAsJsonAsync<SquidexEntities<TEntity, TData>>();
        }

        public async Task<TEntity> GetAsync(string id, QueryContext context = null)
        {
            Guard.NotNullOrEmpty(id, nameof(id));
;
            var response = await RequestAsync(HttpMethod.Get, $"{id}/", context: context);

            return await response.Content.ReadAsJsonAsync<TEntity>();
        }

        public async Task<TEntity> CreateAsync(TData data, bool publish = false)
        {
            Guard.NotNull(data, nameof(data));

            var response = await RequestAsync(HttpMethod.Post, $"?publish={publish}", data.ToContent());

            return await response.Content.ReadAsJsonAsync<TEntity>();
        }

        public Task UpdateAsync(string id, TData data)
        {
            Guard.NotNull(data, nameof(data));
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Put, $"{id}/", data.ToContent());
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

            return RequestAsync(HttpMethod.Put, $"{id}/publish/");
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

            return RequestAsync(HttpMethod.Put, $"{id}/unpublish/");
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

            return RequestAsync(HttpMethod.Put, $"{id}/archive/");
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

            return RequestAsync(HttpMethod.Put, $"{id}/restore/");
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

            return RequestAsync(HttpMethod.Delete, $"{id}/");
        }

        public async Task DeleteAsync(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            await DeleteAsync(entity.Id);

            entity.MarkAsUpdated();
        }

        private async Task<HttpResponseMessage> RequestAsync(HttpMethod method, string path = "", HttpContent content = null, QueryContext context = null)
        {
            var uri = new Uri(serviceUrl, $"api/content/{applicationName}/{schemaName}/{path}");

            var requestToken = await authenticator.GetBearerTokenAsync();
            var request = BuildRequest(method, content, uri, requestToken);

            if (context != null)
            {
                if (context.IsFlatten)
                {
                    request.Headers.TryAddWithoutValidation("X-Flatten", "true");
                }

                if (context.Languages != null)
                {
                    var languages = string.Join(", ", context.Languages.Where(x => !string.IsNullOrWhiteSpace(x)));

                    if (!string.IsNullOrWhiteSpace(languages))
                    {
                        request.Headers.TryAddWithoutValidation("X-Languages", languages);
                    }
                }
            }

            var response = await SquidexHttpClient.Instance.SendAsync(request);

            await EnsureResponseIsValidAsync(response, requestToken);

            return response;
        }

        private static HttpRequestMessage BuildRequest(HttpMethod method, HttpContent content, Uri uri, string requestToken)
        {
            var request = new HttpRequestMessage(method, uri);

            if (content != null)
            {
                request.Content = content;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", requestToken);

            return request;
        }

        private async Task EnsureResponseIsValidAsync(HttpResponseMessage response, string token)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await authenticator.RemoveTokenAsync(token);
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new SquidexException("The app, schema or entity does not exist.");
                }

                var message = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "Squidex API failed with internal error.";
                }
                else
                {
                    message = $"Squidex Request failed: {message}";
                }

                throw new SquidexException(message);
            }
        }
    }
}
