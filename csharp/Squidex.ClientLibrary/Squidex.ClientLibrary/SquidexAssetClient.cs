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
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.ClientLibrary
{
    [Obsolete]
    public sealed class SquidexAssetClient : SquidexClientBase
    {
        public SquidexAssetClient(string applicationName, HttpClient httpClient)
            : base(applicationName, httpClient)
        {
        }

        public Task<AssetEntities> GetAssetsAsync(ODataQuery query = null)
        {
            var queryString = query?.ToQuery(false) ?? string.Empty;

            return RequestJsonAsync<AssetEntities>(HttpMethod.Get, BuildAppAssetsUrl(queryString));
        }

        public Task<Stream> GetAssetContentAsync(string id, int? version = null, int? width = null, int? height = null, string mode = null, int? quality = null)
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

            if (quality.HasValue)
            {
                queries.Add($"quality={quality.Value}");
            }

            var queryString = string.Join("&", queries);

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                queryString = "?" + queryString;
            }

            return RequestStreamAsync(HttpMethod.Get, BuildAssetsUrl($"{id}/{queryString}"));
        }

        public Task<Asset> GetAssetAsync(string id)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestJsonAsync<Asset>(HttpMethod.Get, BuildAppAssetsUrl(id));
        }

        public Task<Asset> CreateAssetAsync(string name, string mimeType, Stream stream, CancellationToken ct = default)
        {
            Guard.NotNull(stream, nameof(stream));

            return RequestJsonAsync<Asset>(HttpMethod.Post, BuildAppAssetsUrl(), stream.ToContent(name, mimeType), ct: ct);
        }

        public Task<Asset> UpdateAssetContentAsync(string id, string name, string mimeType, Stream stream, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));
            Guard.NotNull(stream, nameof(stream));

            return RequestJsonAsync<Asset>(HttpMethod.Put, BuildAppAssetsUrl($"{id}/content"), stream.ToContent(name, mimeType), ct: ct);
        }

        public Task<Asset> UpdateAssetAsync(string id, UpdateAssetRequest update, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));
            Guard.NotNull(update, nameof(update));

            return RequestJsonAsync<Asset>(HttpMethod.Put, BuildAppAssetsUrl(id), update.ToContent(), ct: ct);
        }

        public Task<Tags> GetAssetsTagsAsync(CancellationToken ct = default)
        {
            return RequestJsonAsync<Tags>(HttpMethod.Get, BuildAppAssetsUrl("tags"), ct: ct);
        }

        public Task DeleteAssetAsync(string id, CancellationToken ct = default)
        {
            Guard.NotNullOrEmpty(id, nameof(id));

            return RequestAsync(HttpMethod.Delete, BuildAppAssetsUrl(id), ct: ct);
        }

        public Task DeleteAssetAsync(Asset asset, CancellationToken ct = default)
        {
            Guard.NotNull(asset, nameof(asset));

            return DeleteAssetAsync(asset.Id, ct);
        }

        private string BuildAssetsUrl(string path = "")
        {
            return $"assets/{path}";
        }

        private string BuildAppAssetsUrl(string path = "")
        {
            return $"apps/{ApplicationName}/assets/{path}";
        }
    }
}
