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
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// The context object to add additonal headers to the request and
    /// change the behavior of the API when querying content items.
    /// </summary>
    /// <remarks>
    /// This class is immutable and creates a new instance when using any method.
    /// </remarks>
    public sealed class QueryContext
    {
        /// <summary>
        /// The default behavior.
        /// </summary>
        public static readonly QueryContext Default = new QueryContext();

        /// <summary>
        /// Gets the asset fields where the ID should be converted to URLs.
        /// </summary>
        /// <value>
        /// The asset fields where the ID should be converted to URLs.
        /// </value>
        public IEnumerable<string> AssetUrlsToResolve { get; private set; }

        /// <summary>
        /// Gets the languages to deliver.
        /// </summary>
        /// <value>
        /// The languages to deliver.
        /// </value>
        public IEnumerable<string> Languages { get; private set; }

        /// <summary>
        /// Gets a value indicating whether content items will be flatten.
        /// </summary>
        /// <value>
        ///   <c>true</c> if content items will be flatten; otherwise, <c>false</c>.
        /// </value>
        public bool IsFlatten { get; private set; }

        /// <summary>
        /// Gets a value indicating whether unpublished content items will be delivered.
        /// </summary>
        /// <value>
        ///   <c>true</c> if unpublished content items will be delivered; otherwise, <c>false</c>.
        /// </value>
        public bool IsUnpublished { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Content CDN will not be used.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the Content CDN will not be used; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotUsingCDN { get; private set; }

        /// <summary>
        /// Gets a value indicating whether fallback handling for undefined field languages will be turned off.
        /// </summary>
        /// <value>
        ///   <c>true</c> if fallback handling for undefined field languages will be turned off; otherwise, <c>false</c>.
        /// </value>
        public bool IsIgnoreFallback { get; private set; }

        private QueryContext()
        {
        }

        /// <summary>
        /// Creates a new copy of the context object and defines whether content items will be flatten.
        /// </summary>
        /// <param name="flatten">if set to <c>true</c> content items will be flatten.</param>
        /// <returns>
        /// The new query context.
        /// </returns>
        public QueryContext Flatten(bool flatten = true)
        {
            return Clone(c => c.IsFlatten = flatten);
        }

        /// <summary>
        /// Creates a new copy of the context object and defines whether unpublished content items will be deliverd.
        /// </summary>
        /// <param name="unpublished">if set to <c>true</c> unpublished content items will be delivered.</param>
        /// <returns>
        /// The new query context.
        /// </returns>
        public QueryContext Unpublished(bool unpublished = true)
        {
            return Clone(c => c.IsUnpublished = unpublished);
        }

        /// <summary>
        /// Creates a new copy of the context object and defines whether the Content CDN will not be used.
        /// </summary>
        /// <param name="value">if set to <c>true</c> the Content CDN will not be used.</param>
        /// <returns>
        /// The new query context.
        /// </returns>
        public QueryContext WithoutCDN(bool value = true)
        {
            return Clone(c => c.IsNotUsingCDN = value);
        }

        /// <summary>
        /// Creates a new copy of the context object and defines which asset fields should be resolved.
        /// </summary>
        /// <param name="urls">The asset fields to resolve.</param>
        /// <returns>
        /// The new query context.
        /// </returns>
        public QueryContext ResolveAssetUrls(params string[] urls)
        {
            Guard.NotNull(urls, nameof(urls));

            return Clone(c => c.AssetUrlsToResolve = urls);
        }

        /// <summary>
        /// Creates a new copy of the context object with the language objects to deliver.
        /// </summary>
        /// <param name="languages">The languages to deliver.</param>
        /// <returns>
        /// The new query context.
        /// </returns>
        public QueryContext WithLanguages(params string[] languages)
        {
            Guard.NotNull(languages, nameof(languages));

            return Clone(c => c.Languages = languages);
        }

        /// <summary>
        /// Creates a new copy of the context object and defines fallback handling for undefined field languages will be turned off.
        /// </summary>
        /// <param name="value">if set to <c>true</c> fallback handling for undefined field languages will be turned off.</param>
        /// <returns>
        /// The new query context.
        /// </returns>
        public QueryContext IgnoreFallback(bool value = true)
        {
            return Clone(c => c.IsIgnoreFallback = value);
        }

        internal void AddToHeaders(HttpRequestHeaders headers)
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
                    headers.TryAddWithoutValidation("X-Resolve-Urls", assetFields);
                }
            }

            if (IsIgnoreFallback)
            {
                headers.TryAddWithoutValidation("X-NoResolveLanguages", "1");
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
