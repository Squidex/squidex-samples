// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.Assets.TusClient;
using Squidex.ClientLibrary.Utils;

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.ClientLibrary;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public partial interface IAssetsClient
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    /// <summary>
    /// Gets the upload progress.
    /// </summary>
    /// <param name="fileId">The file id of the upload.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// The upload progress in bytes.
    /// </returns>
    Task<long> GetUploadProgressAsync(string fileId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload a new asset using tus protocol.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="options">Optional arguments.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// Task for completion.
    /// </returns>
    /// <exception cref="SquidexException">A server side error occurred.</exception>
    Task UploadAssetAsync(FileParameter file, AssetUploadOptions options = default,
        CancellationToken cancellationToken = default);

    /// <summary>Get assets.</summary>
    /// <param name="query">The optional asset query.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// Assets returned.
    /// </returns>
    /// <exception cref="SquidexException">A server side error occurred.</exception>
    Task<AssetsDto> GetAssetsAsync(AssetQuery? query = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get assets.
    /// </summary>
    /// <param name="callback">The callback that is invoke for each asset.</param>
    /// <param name="batchSize">The number of assets per request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// Assets returned.
    /// </returns>
    /// <exception cref="SquidexException">A server side error occurred.</exception>
    Task GetAllAsync(Func<AssetDto, Task> callback, int batchSize = 200,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get assets.
    /// </summary>
    /// <param name="callback">The callback that is invoke for each asset.</param>
    /// <param name="query">The optional asset query.</param>
    /// <param name="batchSize">The number of assets per request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>
    /// Assets returned.
    /// </returns>
    /// <exception cref="SquidexException">A server side error occurred.</exception>
    Task GetAllByQueryAsync(Func<AssetDto, Task> callback, AssetQuery? query = null, int batchSize = 200,
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
    public async Task<long> GetUploadProgressAsync(string fileId,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(fileId, nameof(fileId));

        var url = $"api/apps/{_options.AppName}/assets/tus/";

        var httpClient = _options.ClientProvider.Get();

        return await httpClient.GetUploadProgressAsync(url, fileId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UploadAssetAsync(FileParameter file, AssetUploadOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(file, nameof(file));

        var url = $"api/apps/{_options.AppName}/assets/tus";

        var httpClient = _options.ClientProvider.Get();

        await httpClient.UploadWithProgressAsync(url, file.ToUploadFile(), options.ToOptions(this), cancellationToken);
    }

    /// <inheritdoc />
    public Task<AssetsDto> GetAssetsAsync(AssetQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        return GetAssetsAsync(query?.ParentId, query?.ToIdString(), query?.ToQueryJson(_options), query?.Top, query?.Skip, query?.OrderBy, query?.Filter, null, null, cancellationToken);
    }

    /// <inheritdoc />
    public Task GetAllAsync(Func<AssetDto, Task> callback, int batchSize = 200,
        CancellationToken cancellationToken = default)
    {
        return GetAllByQueryAsync(callback, null, batchSize, cancellationToken);
    }

    /// <inheritdoc />
    public async Task GetAllByQueryAsync(Func<AssetDto, Task> callback, AssetQuery? query = null, int batchSize = 200,
        CancellationToken cancellationToken = default)
    {
        Guard.Between(batchSize, 10, 10_000, nameof(batchSize));
        Guard.NotNull(callback, nameof(callback));

        query ??= new AssetQuery();
        query.Top = batchSize;
        query.Skip = 0;

        var added = new HashSet<string>();
        do
        {
            var isAnyAdded = false;

            var getResult = await GetAssetsAsync(query, cancellationToken);

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
