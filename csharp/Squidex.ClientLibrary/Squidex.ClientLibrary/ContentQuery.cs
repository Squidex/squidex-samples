// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Squidex.ClientLibrary
{
    public class ContentQuery
    {
        public long? Skip { get; set; }

        public long? Top { get; set; }

        public string Filter { get; set; }

        public string OrderBy { get; set; }

        public string Search { get; set; }

        public object JsonQuery { get; set; }

        public HashSet<string> Ids { get; set; }

        public string ToQuery(bool supportsSearch)
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
            else if (!string.IsNullOrWhiteSpace(Filter))
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

    [Obsolete]
    public sealed class ODataQuery : ContentQuery
    {
    }
}
