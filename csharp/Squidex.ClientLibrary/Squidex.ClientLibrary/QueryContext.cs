// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Squidex.ClientLibrary
{
    public sealed class QueryContext
    {
        public static readonly QueryContext Default = new QueryContext();

        public IEnumerable<string> AssetUrlsToResolve { get; private set; }

        public IEnumerable<string> Languages { get; private set; }

        public bool IsFlatten { get; private set; }

        public bool IsUnpublished { get; private set; }

        private QueryContext()
        {
        }

        public QueryContext Flatten(bool flatten = true)
        {
            return Clone(c => c.IsFlatten = flatten);
        }

        public QueryContext Unpublished(bool unpublished = true)
        {
            return Clone(c => c.IsUnpublished = unpublished);
        }

        public QueryContext ResolveAssetUrls(params string[] urls)
        {
            Guard.NotNull(urls, nameof(urls));

            return Clone(c => c.AssetUrlsToResolve = urls);
        }

        public QueryContext WithLanguages(params string[] languages)
        {
            Guard.NotNull(languages, nameof(languages));

            return Clone(c => c.Languages = languages);
        }

        public void AddToHeaders(HttpRequestHeaders headers)
        {
            Guard.NotNull(headers, nameof(headers));

            if (IsFlatten)
            {
                headers.TryAddWithoutValidation("X-Flatten", "true");
            }

            if (IsUnpublished)
            {
                headers.TryAddWithoutValidation("X-Unpublished", "true");
            }

            if (Languages != null)
            {
                var languages = string.Join(", ", Languages.Where(x => !string.IsNullOrWhiteSpace(x)));

                if (!string.IsNullOrWhiteSpace(languages))
                {
                    headers.TryAddWithoutValidation("X-Languages", languages);
                }
            }

            if (AssetUrlsToResolve != null)
            {
                var assetFields = string.Join(", ", AssetUrlsToResolve.Where(x => !string.IsNullOrWhiteSpace(x)));

                if (!string.IsNullOrWhiteSpace(assetFields))
                {
                    headers.TryAddWithoutValidation("X-AssetFields", assetFields);
                }
            }
        }

        private QueryContext Clone(Action<QueryContext> updater)
        {
            var result = (QueryContext)MemberwiseClone();

            updater(result);

            return result;
        }
    }
}
