// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Squidex.ClientLibrary
{
    public sealed class SquidexAssetClient : SquidexClientBase
    {
        public SquidexAssetClient(string applicationName, HttpClient httpClient)
            : base(applicationName, httpClient)
        {
        }

        public async Task<AssetEntities> GetAssetsAsync(string filter = null, string ids = null, int? skip = null, int? top = null)
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

            if (!string.IsNullOrEmpty(filter))
            {
                queries.Add($"$filter={filter}");
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

            var response = await RequestAsync(HttpMethod.Get, BuildAppAssetsUrl(queryString));

            return await response.Content.ReadAsJsonAsync<AssetEntities>();
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

        public async Task<Asset> CreateAssetAsync(string contentName, string contentMimeType, Stream stream)
        {
            var request = BuildRequest(contentName, contentMimeType, stream);
            var response = await RequestAsync(HttpMethod.Post, BuildAppAssetsUrl(), request);

            return await response.Content.ReadAsJsonAsync<Asset>();
        }

        public async Task<Asset> UpdateAssetContentAsync(string id, string contentName, string contentMimeType, Stream stream)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            var request = BuildRequest(contentName, contentMimeType, stream);
            var response = await RequestAsync(HttpMethod.Put, BuildAppAssetsUrl($"{id}/content"), request);

            return await response.Content.ReadAsJsonAsync<Asset>();
        }

        public async Task<HttpStatusCode> UpdateAssetAsync(string id, UpdateAssetRequest data)
        {
            var jsonData = JsonConvert.SerializeObject(data);

            var request = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await RequestAsync(HttpMethod.Put, BuildAppAssetsUrl(id), request);

            return response.StatusCode;
        }

        public async Task<Dictionary<string, int>> GetAssetsTagsAsync()
        {
            var request = await RequestAsync(HttpMethod.Get, BuildAppAssetsUrl("tags"));
            var response = await request.Content.ReadAsJsonAsync<Dictionary<string, int>>();

            return response;
        }

        public async Task DeleteAssetAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            await RequestAsync(HttpMethod.Delete, BuildAppAssetsUrl(id));
        }

        private string BuildAssetsUrl(string path = "")
        {
            return $"assets/{path}";
        }

        private string BuildAppAssetsUrl(string path = "")
        {
            return $"apps/{ApplicationName}/assets/{path}";
        }

        private static MultipartFormDataContent BuildRequest(string contentName, string contentMimeType, Stream stream)
        {
            Guard.NotNullOrEmpty(contentName, nameof(contentName));
            Guard.NotNullOrEmpty(contentMimeType, nameof(contentMimeType));
            Guard.NotNull(stream, nameof(stream));

            var streamContent = new StreamContent(stream);

            streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentMimeType);

            var requestContent = new MultipartFormDataContent
            {
                { streamContent, "file", contentName }
            };

            return requestContent;
        }
    }
}
