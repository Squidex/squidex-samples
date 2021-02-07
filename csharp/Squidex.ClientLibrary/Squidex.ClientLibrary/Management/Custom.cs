// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Utils;

#pragma warning disable RECS0096 // Type parameter is never used
#pragma warning disable SA1629 // Documentation text should end with a period

namespace Squidex.ClientLibrary.Management
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class ErrorDto
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Message);

            if (Details?.Count > 0)
            {
                if (!Message.EndsWith(".", StringComparison.OrdinalIgnoreCase) &&
                    !Message.EndsWith(":", StringComparison.OrdinalIgnoreCase) &&
                    !Message.EndsWith(",", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(":");
                }

                sb.Append(" ");
                sb.Append(string.Join(", ", Details));
            }

            return sb.ToString();
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class SquidexManagementException<TResult>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Result}\n{base.ToString()}";
        }
    }

    /// <summary>
    /// Represents an asset query.
    /// </summary>
    public class AssetQuery
    {
        /// <summary>
        /// Gets or sets the IDs of the assets to retrieve.
        /// </summary>
        /// <value>
        /// The IDs of the assets to retrieve.
        /// </value>
        /// <remarks>
        /// If this option is provided all other properties are ignored.
        /// </remarks>
        public List<string> Ids { get; set; }

        /// <summary>
        /// Gets or sets the JSON query.
        /// </summary>
        /// <value>
        /// The JSON query.
        /// </value>
        /// <remarks>
        /// Do not use this property in combination with OData properties.
        /// </remarks>
        public object Query { get; set; }

        /// <summary>
        /// Gets or sets the OData argument to define the number of assets to retrieve (<code>$top</code>).
        /// </summary>
        /// <value>
        /// The the number of assets to retrieve.
        /// </value>
        /// <remarks>
        /// Use this property to implement pagination but not in combination with <see cref="Query"/> property.
        /// </remarks>
        public int? Top { get; set; }

        /// <summary>
        /// Gets or sets the OData argument to define number of assets to skip (<code>$skip</code>).
        /// </summary>
        /// <value>
        /// The the number of assets to skip.
        /// </value>
        /// <remarks>
        /// Use this property to implement pagination but not in combination with <see cref="Query"/> property.
        /// </remarks>
        public int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the OData order argument (<code>$orderby</code>).
        /// </summary>
        /// <value>
        /// The OData order argument.
        /// </value>
        /// <remarks>
        /// Do not use this property in combination with <see cref="Query"/> property.
        /// </remarks>
        public string OrderBy { get; set; }

        /// <summary>
        /// Gets or sets the OData filter argument (<code>$filter</code>).
        /// </summary>
        /// <value>
        /// The OData filter argument.
        /// </value>
        /// <remarks>
        /// Do not use this property in combination with <see cref="Query"/> property.
        /// </remarks>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the optional folder ID.
        /// </summary>
        /// <value>
        /// The parent optional folder ID.
        /// </value>
        public string ParentId { get; set; }

        internal string ToQueryJson()
        {
            if (Query == null)
            {
                return null;
            }

            return Query.ToJson();
        }

        internal string ToIdString()
        {
            if (Ids == null)
            {
                return null;
            }

            return string.Join(",", Ids);
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial interface IAssetsClient
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get assets.</summary>
        /// <param name="app">The name of the app.</param>
        /// <param name="query">The optional asset query.</param>
        /// <returns>Assets returned.</returns>
        /// <exception cref="SquidexManagementException">A server side error occurred.</exception>
        Task<AssetsDto> GetAssetsAsync(string app, AssetQuery query = null, System.Threading.CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get assets.</summary>
        /// <param name="app">The name of the app.</param>
        /// <param name="callback">The callback that is invoke for each asset.</param>
        /// <param name="batchSize">The number of assets per request.</param>
        /// <returns>Assets returned.</returns>
        /// <exception cref="SquidexManagementException">A server side error occurred.</exception>
        Task GetAllAsync(string app, Func<AssetDto, Task> callback, int batchSize = 200, System.Threading.CancellationToken cancellationToken = default);
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class AssetsClient
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get assets.</summary>
        /// <param name="app">The name of the app.</param>
        /// <param name="query">The optional asset query.</param>
        /// <returns>Assets returned.</returns>
        /// <exception cref="SquidexManagementException">A server side error occurred.</exception>
        public Task<AssetsDto> GetAssetsAsync(string app, AssetQuery query = null, System.Threading.CancellationToken cancellationToken = default)
        {
            return GetAssetsAsync(app,  query?.Top, query?.Skip, query?.OrderBy, query?.Filter, query?.ParentId, query?.ToIdString(), query?.ToQueryJson(), cancellationToken);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get assets.</summary>
        /// <param name="app">The name of the app.</param>
        /// <param name="callback">The callback that is invoke for each asset.</param>
        /// <param name="batchSize">The number of assets per request.</param>
        /// <returns>Assets returned.</returns>
        /// <exception cref="SquidexManagementException">A server side error occurred.</exception>
        public async Task GetAllAsync(string app, Func<AssetDto, Task> callback, int batchSize = 200, System.Threading.CancellationToken cancellationToken = default)
        {
            var query = new AssetQuery { Top = batchSize, Skip = 0 };
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
