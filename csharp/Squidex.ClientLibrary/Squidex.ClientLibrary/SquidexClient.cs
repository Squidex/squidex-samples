// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
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

        public async Task<SquidexEntities<TEntity, TData>> GetAsync(long? skip = null, long? top = null, string filter = null, string orderBy = null, string search = null)
        {
            using (var httpClient = new HttpClient())
            {
                await SetBearerTokenAsync(httpClient);

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

                var requestUri = BuildUrl(query);
                var response = await httpClient.GetAsync(requestUri);

                await EnsureResponseIsValidAsync(response);

                return await response.Content.ReadAsJsonAsync<SquidexEntities<TEntity, TData>>();
            }
        }
 
        public async Task<TEntity> GetAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            using (var httpClient = new HttpClient())
            {
                await SetBearerTokenAsync(httpClient);

                var requestUri = BuildUrl($"{id}/");
                var response = await httpClient.GetAsync(requestUri);

                await EnsureResponseIsValidAsync(response);

                return await response.Content.ReadAsJsonAsync<TEntity>();
            }
        }

        public async Task<TEntity> CreateAsync(string id, TData data)
        {
            Guard.NotNull(data, nameof(data));
            Guard.NotNullOrEmpty(id, nameof(id));

            using (var httpClient = new HttpClient())
            {
                await SetBearerTokenAsync(httpClient);

                var requestUri = BuildUrl($"{id}/");
                var response = await httpClient.PutAsJsonAsync(requestUri, data);

                await EnsureResponseIsValidAsync(response);

                return await response.Content.ReadAsJsonAsync<TEntity>();
            }
        }

        public async Task UpdateAsync(string id, TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));
            Guard.NotNullOrEmpty(id, nameof(id));

            using (var httpClient = new HttpClient())
            {
                await SetBearerTokenAsync(httpClient);

                var requestUri = BuildUrl($"{id}/");
                var response = await httpClient.PutAsJsonAsync(requestUri, entity.Data);

                await EnsureResponseIsValidAsync(response);

                entity.MarkAsUpdated();
            }
        }

        public async Task PublishAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            using (var httpClient = new HttpClient())
            {
                await SetBearerTokenAsync(httpClient);

                var requestUri = BuildUrl($"{id}/publish/");
                var response = await httpClient.PutAsync(requestUri, new ByteArrayContent(new byte[0]));

                await EnsureResponseIsValidAsync(response);
            }
        }

        public async Task UnpublishAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            using (var httpClient = new HttpClient())
            {
                await SetBearerTokenAsync(httpClient);

                var requestUri = BuildUrl($"{id}/unpublish/");
                var response = await httpClient.PutAsync(requestUri, new ByteArrayContent(new byte[0]));

                await EnsureResponseIsValidAsync(response);
            }
        }

        public async Task DeleteAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            using (var httpClient = new HttpClient())
            {
                await SetBearerTokenAsync(httpClient);

                var requestUri = BuildUrl($"{id}/publish/");
                var response = await httpClient.DeleteAsync(requestUri);

                await EnsureResponseIsValidAsync(response);
            }
        }

        private static async Task EnsureResponseIsValidAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
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

        private Uri BuildUrl(string path = "")
        {
            return new Uri(serviceUrl, $"api/content/{applicationName}/{schemaName}/{path}");
        }

        private async Task SetBearerTokenAsync(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await authenticator.GetBearerTokenAsync());
        }
    }
}
