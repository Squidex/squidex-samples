// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Configuration;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary;

/// <summary>
/// Default implementation of the <see cref="IContentsClient{TEntity, TData}"/> interface.
/// </summary>
/// <typeparam name="TEntity">The type for the content entity.</typeparam>
/// <typeparam name="TData">The type that represents the data structure.</typeparam>
/// <seealso cref="SquidexClientBase" />
/// <seealso cref="IContentsClient{TEntity, TData}" />
public sealed class ContentsClient<TEntity, TData> : SquidexClientBase, IContentsClient<TEntity, TData> where TEntity : Content<TData> where TData : class, new()
{
    /// <inheritdoc/>
    public string SchemaName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentsClient{TEntity, TData}"/> class
    /// with the name of the schema, the options from the <see cref="SquidexClient"/> and the HTTP client.
    /// </summary>
    /// <param name="options">The options from the <see cref="SquidexClient"/>. Cannot be null.</param>
    /// <param name="schemaName">Name of the schema. Cannot be null or empty.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="schemaName"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="schemaName"/> is empty.</exception>
    public ContentsClient(SquidexOptions options, string schemaName)
        : base(options)
    {
        Guard.NotNullOrEmpty(schemaName, nameof(schemaName));

        SchemaName = schemaName;
    }

    /// <inheritdoc/>
    public async Task GetAllAsync(Func<TEntity, Task> callback, int batchSize = 200, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.Between(batchSize, 10, 10_000, nameof(batchSize));
        Guard.NotNull(callback, nameof(callback));

        var query = new ContentQuery { Top = batchSize };

        context = (context ?? QueryContext.Default).WithoutTotal();

        var added = new HashSet<string>();
        do
        {
            var isAnyAdded = false;

            var getResult = await GetAsync(query, context, ct);

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
        while (!ct.IsCancellationRequested);
    }

    /// <inheritdoc/>
    public async Task StreamAllAsync(Func<TEntity, Task> callback, int skip = 0, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.Between(skip, 0, int.MaxValue, nameof(skip));
        Guard.NotNull(callback, nameof(callback));

        context = (context ?? QueryContext.Default).WithoutTotal();

        var httpClient = Options.ClientProvider.Get();

        using (var request = BuildRequest(HttpMethod.Get, BuildUrl($"stream?skip={skip}", false), null, context))
        {
            using (var response = await httpClient.SendAsync(request, ct))
            {
                await EnsureResponseIsValidAsync(response);

#if NETSTANDARD2_0 || NETCOREAPP3_1
                using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
#else
                using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync(ct)))
#endif
                {
                    string? line;
#if NET7_0_OR_GREATER
                    while ((line = await reader.ReadLineAsync(ct)) != null)
#else
                    while ((line = await reader.ReadLineAsync()) != null)
#endif
                    {
                        ct.ThrowIfCancellationRequested();

                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        const string Prefix = "data: ";

                        if (!line.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new SquidexException("Line does not start with data prefix.", 0, null);
                        }

#pragma warning disable IDE0057 // Use range operator
                        var contentJson = line.Substring(Prefix.Length);
#pragma warning restore IDE0057 // Use range operator
                        var contentItem = contentJson.FromJson<TEntity>();

                        await callback(contentItem);
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    public Task<ContentsResult<TEntity, TData>> GetAsync(ContentQuery? query = null, QueryContext? context = null,
         CancellationToken ct = default)
    {
        var q = query?.ToQuery(true) ?? string.Empty;

        return RequestJsonAsync<ContentsResult<TEntity, TData>>(HttpMethod.Get, BuildUrl(q, true, context), null, context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> GetAsync(string id, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));

        return RequestJsonAsync<TEntity>(HttpMethod.Get, BuildUrl($"{id}/", true, context), null, context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> GetAsync(string id, long version, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));

        return RequestJsonAsync<TEntity>(HttpMethod.Get, BuildUrl($"{id}/?version={version}", true, context), null, context, ct);
    }

    /// <inheritdoc/>
    public Task<ContentsResult<TEntity, TData>> GetReferencingAsync(TEntity entity, ContentQuery? query = null, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(entity, nameof(entity));

        return GetReferencingAsync(entity.Id, query, context, ct);
    }

    /// <inheritdoc/>
    public Task<ContentsResult<TEntity, TData>> GetReferencingAsync(string id, ContentQuery? query = null, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));

        var q = query?.ToQuery(true) ?? string.Empty;

        return RequestJsonAsync<ContentsResult<TEntity, TData>>(HttpMethod.Get, BuildUrl($"{id}/referencing{q}", true, context), null, context, ct);
    }

    /// <inheritdoc/>
    public Task<ContentsResult<TEntity, TData>> GetReferencesAsync(TEntity entity, ContentQuery? query = null, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(entity, nameof(entity));

        return GetReferencesAsync(entity.Id, query, context, ct);
    }

    /// <inheritdoc/>
    public Task<ContentsResult<TEntity, TData>> GetReferencesAsync(string id, ContentQuery? query = null, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));

        var q = query?.ToQuery(true) ?? string.Empty;

        return RequestJsonAsync<ContentsResult<TEntity, TData>>(HttpMethod.Get, BuildUrl($"{id}/references{q}", true, context), null, context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> CreateAsync(TData data, ContentCreateOptions options = default, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(data, nameof(data));

        return RequestJsonAsync<TEntity>(HttpMethod.Post, BuildUrl($"?publish={options.Publish}&id={options.Id ?? string.Empty}", false), data.ToContent(), context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> CreateDraftAsync(string id, ContentCreateDraftOptions options = default, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));

        return RequestJsonAsync<TEntity>(HttpMethod.Post, BuildUrl($"{id}/draft", false), null, context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> CreateDraftAsync(TEntity entity, ContentCreateDraftOptions options = default, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(entity, nameof(entity));

        return CreateDraftAsync(entity.Id, default, context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> ChangeStatusAsync(string id, ChangeStatus request, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(id, nameof(id));
        Guard.NotNull(request, nameof(request));

        return RequestJsonAsync<TEntity>(HttpMethod.Put, BuildUrl($"{id}/status", false), request.ToContent(), context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> ChangeStatusAsync(TEntity entity, ChangeStatus status, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(entity, nameof(entity));

        return ChangeStatusAsync(entity.Id, status, context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> PatchAsync<TPatch>(string id, TPatch patch, ContentPatchOptions options = default, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));
        Guard.NotNull(patch, nameof(patch));

        return RequestJsonAsync<TEntity>(HttpMethodEx.Patch, BuildUrl($"{id}/", false), patch.ToContent(), context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> PatchAsync<TPatch>(TEntity entity, TPatch patch, ContentPatchOptions options = default, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(entity, nameof(entity));

        return PatchAsync(entity.Id, patch, options, context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> UpsertAsync(string id, TData data, ContentUpsertOptions options = default, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));
        Guard.NotNull(data, nameof(data));

        return RequestJsonAsync<TEntity>(HttpMethod.Post, BuildUrl($"{id}?publish={options.Publish}&patch={options.Patch}", false), data.ToContent(), context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> UpsertAsync(TEntity entity, ContentUpsertOptions options = default, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(entity, nameof(entity));

        return UpsertAsync(entity.Id, entity.Data, options, context, ct);
    }

    /// <inheritdoc/>
    public async Task<TEntity> UpdateAsync(string id, TData data, ContentUpdateOptions options = default, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));
        Guard.NotNull(data, nameof(data));

        return await RequestJsonAsync<TEntity>(HttpMethod.Put, BuildUrl($"{id}", false), data.ToContent(), context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> UpdateAsync(TEntity entity, ContentUpdateOptions options = default, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(entity, nameof(entity));

        return UpdateAsync(entity.Id, entity.Data, default, context, ct);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(string id, ContentDeleteOptions options = default,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));

        return RequestAsync(HttpMethod.Delete, BuildUrl($"{id}?permanent={options.Permanent}&checkReferrers={options.CheckReferrers}", false), null, null, ct);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(TEntity entity, ContentDeleteOptions options = default,
         CancellationToken ct = default)
    {
        Guard.NotNull(entity, nameof(entity));

        await DeleteAsync(entity.Id, options, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> DeleteDraftAsync(string id, ContentDeleteDraftOptions options = default, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNullOrEmpty(id, nameof(id));

        return RequestJsonAsync<TEntity>(HttpMethod.Delete, BuildUrl($"{id}/draft", false), null, context, ct);
    }

    /// <inheritdoc/>
    public Task<TEntity> DeleteDraftAsync(TEntity entity, ContentDeleteDraftOptions options = default, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(entity, nameof(entity));

        return DeleteDraftAsync(entity.Id, options, context, ct);
    }

    /// <inheritdoc/>
    public Task<List<BulkResult>> BulkUpdateAsync(BulkUpdate update,
         CancellationToken ct = default)
    {
        Guard.NotNull(update, nameof(update));

        return RequestJsonAsync<List<BulkResult>>(HttpMethod.Post, BuildUrl("bulk", false), update.ToContent(), null, ct);
    }

    private string BuildUrl(string path, bool query, QueryContext? context = null)
    {
        if (ShouldUseCDN(query, context))
        {
            return $"{Options.ContentCDN}{Options.AppName}/{SchemaName}/{path}";
        }
        else
        {
            return $"api/content/{Options.AppName}/{SchemaName}/{path}";
        }
    }

    private bool ShouldUseCDN(bool query, QueryContext? context)
    {
        return query && !string.IsNullOrWhiteSpace(Options.ContentCDN) && context?.IsNotUsingCDN != true;
    }
}
