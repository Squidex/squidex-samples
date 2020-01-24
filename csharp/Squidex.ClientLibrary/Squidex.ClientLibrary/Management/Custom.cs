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

#pragma warning disable RECS0096 // Type parameter is never used

namespace Squidex.ClientLibrary.Management
{
    public partial class ErrorDto
    {
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Message);

            if (Details?.Count > 0)
            {
                sb.Append(": ");
                sb.Append(string.Join(", ", Details));
            }

            return sb.ToString();
        }
    }

    public partial class SquidexManagementException<TResult>
    {
        public override string ToString()
        {
            return string.Format("{0}\n{1}", Result, base.ToString());
        }
    }

    public class AssetQuery
    {
        public List<Guid> Ids { get; set; }

        public object Query { get; set; }

        public int? Top { get; set; }

        public int? Skip { get; set; }

        public string OrderBy { get; set; }

        public string Filter { get; set; }

        public Guid? ParentId { get; set; }

        public string ToQueryJson()
        {
            if (Query == null)
            {
                return null;
            }

            return Query.ToJson();
        }

        public string ToIdString()
        {
            if (Ids == null)
            {
                return null;
            }

            return string.Join(",", Ids);
        }
    }

    public partial interface IAssetsClient
    {
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get assets.</summary>
        /// <param name="app">The name of the app.</param>
        /// <param name="query">The optional asset query.</param>
        /// <returns>Assets returned.</returns>
        /// <exception cref="SquidexManagementException">A server side error occurred.</exception>
        Task<AssetsDto> GetAssetsAsync(string app, AssetQuery query = null, System.Threading.CancellationToken cancellationToken = default);
    }

    public partial class AssetsClient
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
    }
}
