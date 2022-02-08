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
using Squidex.Assets;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.Management
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial interface IAssetsClient
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Gets the upload progress.
        /// </summary>
        /// <param name="app">The name of the app.</param>
        /// <param name="fileId">The file id of the upload.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The upload progress in bytes.
        /// </returns>
        Task<long> GetUploadProgressAsync(string app, string fileId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Upload a new asset using tus protocol.
        /// </summary>
        /// <param name="app">The name of the app.</param>
        /// <param name="file">The file to upload.</param>
        /// <param name="options">Optional arguments.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// Task for completion.
        /// </returns>
        /// <exception cref="SquidexManagementException">A server side error occurred.</exception>
        Task UploadAssetAsync(string app, FileParameter file, AssetUploadOptions options = default,
            CancellationToken cancellationToken = default);

        /// <summary>Get assets.</summary>
        /// <param name="app">The name of the app.</param>
        /// <param name="query">The optional asset query.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// Assets returned.
        /// </returns>
        /// <exception cref="SquidexManagementException">A server side error occurred.</exception>
        Task<AssetsDto> GetAssetsAsync(string app, AssetQuery? query = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get assets.
        /// </summary>
        /// <param name="app">The name of the app.</param>
        /// <param name="callback">The callback that is invoke for each asset.</param>
        /// <param name="batchSize">The number of assets per request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// Assets returned.
        /// </returns>
        /// <exception cref="SquidexManagementException">A server side error occurred.</exception>
        Task GetAllAsync(string app, Func<AssetDto, Task> callback, int batchSize = 200,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get assets.
        /// </summary>
        /// <param name="app">The name of the app.</param>
        /// <param name="callback">The callback that is invoke for each asset.</param>
        /// <param name="query">The optional asset query.</param>
        /// <param name="batchSize">The number of assets per request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// Assets returned.
        /// </returns>
        /// <exception cref="SquidexManagementException">A server side error occurred.</exception>
        Task GetAllByQueryAsync(string app, Func<AssetDto, Task> callback, AssetQuery? query = null, int batchSize = 200,
            CancellationToken cancellationToken = default);
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class AssetsClient
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public virtual async Task<(T, string)> ReadObjectResponseCoreAsync<T>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            CancellationToken cancellationToken)
        {
            var result = await ReadObjectResponseAsync<T>(response, headers, cancellationToken);

            return (result.Object, result.Text);
        }

        /// <inheritdoc />
        public Task<long> GetUploadProgressAsync(string app, string fileId,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(fileId, nameof(fileId));

            var url = $"api/apps/{app}/assets/tus/";

            return _httpClient.GetUploadProgressAsync(url, fileId, cancellationToken);
        }

        /// <inheritdoc />
        public Task UploadAssetAsync(string app, FileParameter file, AssetUploadOptions options = default,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(file, nameof(file));

            var url = $"api/apps/{app}/assets/tus";

            return _httpClient.UploadWithProgressAsync(url, file.ToUploadFile(), options.ToOptions(file, this), cancellationToken);
        }

        /// <inheritdoc />
        public Task<AssetsDto> GetAssetsAsync(string app, AssetQuery? query = null,
            CancellationToken cancellationToken = default)
        {
            return GetAssetsAsync(app, query?.Top, query?.Skip, query?.OrderBy, query?.Filter, query?.ParentId, query?.ToIdString(), query?.ToQueryJson(), cancellationToken);
        }

        /// <inheritdoc />
        public Task GetAllAsync(string app, Func<AssetDto, Task> callback, int batchSize = 200,
            CancellationToken cancellationToken = default)
        {
            return GetAllByQueryAsync(app, callback, null, batchSize, cancellationToken);
        }

        /// <inheritdoc />
        public async Task GetAllByQueryAsync(string app, Func<AssetDto, Task> callback, AssetQuery? query = null, int batchSize = 200,
            CancellationToken cancellationToken = default)
        {
            Guard.Between(batchSize, 10, 10_000, nameof(batchSize));
            Guard.NotNull(callback, nameof(callback));

            if (query == null)
            {
                query = new AssetQuery();
            }

            query.Top = batchSize;
            query.Skip = 0;

            var added = new HashSet<string>();
            do
            {
                var isAnyAdded = false;

                var getResult = await GetAssetsAsync(app, query, cancellationToken);

                foreach (var item in getResult.Items)
                {
                    if (added.Add(item.Id))
                    {
                        await callback(item);

                        isAnyAdded = true;
                    }
                }

                if (!isAnyAdded)
                {
                    break;
                }

                query.Skip = added.Count;
            }
            while (!cancellationToken.IsCancellationRequested);
        }
    }
}
