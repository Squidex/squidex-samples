// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Utils;

#pragma warning disable SA1629 // Documentation text should end with a period

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// Represents a content query.
    /// </summary>
    public class ContentQuery
    {
        /// <summary>
        /// Gets or sets the IDs of the content items to retrieve.
        /// </summary>
        /// <value>
        /// The IDs of the content items to retrieve.
        /// </value>
        /// <remarks>
        /// If this option is provided all other properties are ignored.
        /// </remarks>
        public HashSet<string>? Ids { get; set; }

        /// <summary>
        /// Gets or sets the JSON query.
        /// </summary>
        /// <value>
        /// The JSON query.
        /// </value>
        /// <remarks>
        /// Do not use this property in combination with OData properties.
        /// </remarks>
        public object? JsonQuery { get; set; }

        /// <summary>
        /// Gets or sets the OData argument to define the number of content items to retrieve (<code>$top</code>).
        /// </summary>
        /// <value>
        /// The the number of content items to retrieve.
        /// </value>
        /// <remarks>
        /// Use this property to implement pagination but not in combination with <see cref="JsonQuery"/> property.
        /// </remarks>
        public int? Top { get; set; }

        /// <summary>
        /// Gets or sets the OData argument to define number of content items to skip (<code>$skip</code>).
        /// </summary>
        /// <value>
        /// The the number of content items to skip.
        /// </value>
        /// <remarks>
        /// Use this property to implement pagination but not in combination with <see cref="JsonQuery"/> property.
        /// </remarks>
        public int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the OData order argument (<code>$orderby</code>).
        /// </summary>
        /// <value>
        /// The OData order argument.
        /// </value>
        /// <remarks>
        /// Do not use this property in combination with <see cref="JsonQuery"/> property.
        /// </remarks>
        public string? OrderBy { get; set; }

        /// <summary>
        /// Gets or sets the OData filter argument (<code>$filter</code>).
        /// </summary>
        /// <value>
        /// The OData filter argument.
        /// </value>
        /// <remarks>
        /// Do not use this property in combination with <see cref="JsonQuery"/> property.
        /// </remarks>
        public string? Filter { get; set; }

        /// <summary>
        /// Gets or sets the OData argument to define number of full text search (<code>$search</code>).
        /// </summary>
        /// <value>
        /// The full text query.
        /// </value>
        public string? Search { get; set; }

        internal string ToQuery(bool supportsSearch)
        {
            var queries = new List<string>();

            if (Skip.HasValue)
            {
                queries.Add($"$skip={Skip.Value}");
            }

            if (Top.HasValue)
            {
                queries.Add($"$top={Top.Value}");
            }

            if (!string.IsNullOrWhiteSpace(OrderBy))
            {
                queries.Add($"$orderby={OrderBy}");
            }

            if (JsonQuery != null)
            {
                queries.Add($"q={JsonQuery.ToJson()}");
            }

            if (Ids != null && Ids.Count > 0)
            {
                queries.Add($"ids={string.Join(",", Ids)}");
            }

            if (!string.IsNullOrWhiteSpace(Search))
            {
                if (!supportsSearch)
                {
                    throw new NotSupportedException("Full text search is not supported.");
                }

                queries.Add($"$search=\"{Search}\"");
            }

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                queries.Add($"$filter={Filter}");
            }

            var queryString = string.Join("&", queries);

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                queryString = "?" + queryString;
            }

            return queryString;
        }
    }
}
