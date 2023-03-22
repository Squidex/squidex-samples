// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;
using Newtonsoft.Json.Linq;
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
public sealed class ContentsSharedClient<TEntity, TData> : SquidexClientBase, IContentsSharedClient<TEntity, TData> where TEntity : Content<TData> where TData : class, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentsSharedClient{TEntity, TData}"/> class
    /// with the name of the schema, the options from the <see cref="SquidexClient"/> and the HTTP client.
    /// </summary>
    /// <param name="options">The options from the <see cref="SquidexClient"/>. Cannot be null.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public ContentsSharedClient(SquidexOptions options)
        : base(options)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GraphQlResponse<TResponse>>> GraphQlAsync<TResponse>(IEnumerable<object> requests, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(requests, nameof(requests));

        var response = await RequestJsonAsync<GraphQlResponse<TResponse>[]>(HttpMethod.Post, BuildUrl("graphql/batch", false, context), requests.ToContent(), context, ct);

        return response;
    }

    /// <inheritdoc/>
    public async Task<TResponse> GraphQlGetAsync<TResponse>(object request, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(request, nameof(request));

        var query = BuildQuery(request);

        var response = await RequestJsonAsync<GraphQlResponse<TResponse>>(HttpMethod.Get, BuildUrl("graphql", true, context) + query, null, context, ct);

        if (response.Errors?.Length > 0)
        {
            throw new SquidexGraphQlException(response.Errors, 400);
        }

        return response.Data;
    }

    /// <inheritdoc/>
    public async Task<TResponse> GraphQlAsync<TResponse>(object request, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(request, nameof(request));

        var response = await RequestJsonAsync<GraphQlResponse<TResponse>>(HttpMethod.Post, BuildUrl("graphql", false, context), request.ToContent(), context, ct);

        if (response.Errors?.Length > 0)
        {
            throw new SquidexGraphQlException(response.Errors, 400);
        }

        return response.Data;
    }

    /// <inheritdoc/>
    public Task<ContentsResult<TEntity, TData>> GetAsync(HashSet<string> ids, QueryContext? context = null,
         CancellationToken ct = default)
    {
        Guard.NotNull(ids, nameof(ids));
        Guard.NotNullOrEmpty(ids, nameof(ids));

        var q = $"?ids={string.Join(",", ids)}";

        return RequestJsonAsync<ContentsResult<TEntity, TData>>(HttpMethod.Get, BuildUrl(q, true, context), null, context, ct);
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
            return $"{Options.ContentCDN}{Options.AppName}/{path}";
        }
        else
        {
            return $"api/content/{Options.AppName}/{path}";
        }
    }

    private static string BuildQuery(object request)
    {
        var parameters = JObject.FromObject(request);

        var queryBuilder = new StringBuilder();

        foreach (var kvp in parameters)
        {
            var value = kvp.Value;

            if (value == null)
            {
                continue;
            }

            if (queryBuilder.Length > 0)
            {
                queryBuilder.Append('&');
            }
            else
            {
                queryBuilder.Append('?');
            }

            queryBuilder.Append(kvp.Key);
            queryBuilder.Append('=');
            queryBuilder.Append(Uri.EscapeDataString(value.ToString()));
        }

        return queryBuilder.ToString();
    }

    private bool ShouldUseCDN(bool query, QueryContext? context)
    {
        return query && !string.IsNullOrWhiteSpace(Options.ContentCDN) && context?.IsNotUsingCDN != true;
    }
}
