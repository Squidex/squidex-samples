// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Squidex.ClientLibrary
{
    public sealed class SquidexAssetClient : SquidexClientBase
    {
        public SquidexAssetClient(Uri serviceUrl, string applicationName, string schemaName, IAuthenticator authenticator)
            : base(serviceUrl, applicationName, schemaName, authenticator)
        {
        }

        public async Task<Asset> UpdateAssetContentAsync(string id, string contentName, string contentMimeType, byte[] contentBytes)
        {
            Guard.NotNullOrEmpty(id, nameof(id));
            Guard.NotNullOrEmpty(contentName, nameof(contentName));
            Guard.NotNullOrEmpty(contentMimeType, nameof(contentMimeType));
            Guard.NotNull(contentBytes, nameof(contentBytes));

            var streamContent = new ByteArrayContent(contentBytes);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentMimeType);

            var requestContent = new MultipartFormDataContent
            {
                { streamContent, "file", contentName }
            };

            var response = await RequestAsync(HttpMethod.Put, BuildAppAssetsUrl($"{id}/content"), requestContent);
            return await response.Content.ReadAsJsonAsync<Asset>();
        }

        public async Task DeleteAssetAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            await RequestAsync(HttpMethod.Delete, BuildAppAssetsUrl(id));
        }

        public async Task<Asset> CreateAssetAsync(string contentName, string contentMimeType, byte[] contentBytes)
        {
            Guard.NotNullOrEmpty(contentName, nameof(contentName));
            Guard.NotNullOrEmpty(contentMimeType, nameof(contentMimeType));
            Guard.NotNull(contentBytes, nameof(contentBytes));

            var streamContent = new ByteArrayContent(contentBytes);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentMimeType);

            var requestContent = new MultipartFormDataContent
            {
                { streamContent, "file", contentName }
            };

            var response = await RequestAsync(HttpMethod.Post, BuildAppAssetsUrl(), requestContent);
            return await response.Content.ReadAsJsonAsync<Asset>();
        }

        public async Task<Stream> GetAssetContentAsync(string id, int? version = null, int? width = null, int? height = null, string mode = null)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            var queries = new List<string> { $"app={ApplicationName}" };

            if (version.HasValue)
            {
                queries.Add($"version={version.Value}");
            }

            if (width.HasValue)
            {
                queries.Add($"width={width.Value}");
            }

            if (height.HasValue)
            {
                queries.Add($"height={height.Value}");
            }

            if (!string.IsNullOrEmpty(mode))
            {
                queries.Add($"mode={mode}");
            }

            var queryString = string.Join("&", queries);
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                queryString = "?" + queryString;
            }

            var response = await RequestAsync(HttpMethod.Get, BuildAssetsUrl($"{id}/{queryString}"));
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<Asset> GetAssetAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            var response = await RequestAsync(HttpMethod.Get, BuildAppAssetsUrl(id));
            return await response.Content.ReadAsJsonAsync<Asset>();
        }

        public async Task<AssetEntities> GetAssetsAsync(string query = null, string mimeTypes = null, string ids = null, int? skip = null, int? take = null)
        {
            var queries = new List<string>();

            if (skip.HasValue)
            {
                queries.Add($"$skip={skip.Value}");
            }

            if (take.HasValue)
            {
                queries.Add($"take={take.Value}");
            }

            if (!string.IsNullOrEmpty(query))
            {
                queries.Add($"$query={query}");
            }

            if (!string.IsNullOrEmpty(mimeTypes))
            {
                queries.Add($"mimeTypes={mimeTypes}");
            }

            if (!string.IsNullOrEmpty(ids))
            {
                queries.Add($"ids={ids}");
            }

            var queryString = string.Join("&", queries);
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                queryString = "?" + queryString;
            }

            var response = await RequestAsync(HttpMethod.Get, BuildAppAssetsUrl(query));
            return await response.Content.ReadAsJsonAsync<AssetEntities>();
        }

        private string BuildAssetsUrl(string path = "")
        {
            return $"api/assets/{path}";
        }

        private string BuildAppAssetsUrl(string path = "")
        {
            return $"api/apps/{ApplicationName}/assets/{path}";
        }
    }
}
