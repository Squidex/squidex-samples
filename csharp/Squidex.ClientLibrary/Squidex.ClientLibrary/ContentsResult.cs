// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// The result set of contents.
    /// </summary>
    /// <typeparam name="TEntity">The type for the content entity.</typeparam>
    /// <typeparam name="TData">The type that represents the data structure.</typeparam>
    /// <seealso cref="Resource" />
    public sealed class ContentsResult<TEntity, TData> : Resource where TEntity : Content<TData> where TData : class, new()
    {
        /// <summary>
        /// Gets the content items.
        /// </summary>
        /// <value>
        /// The content items.
        /// </value>
        public List<TEntity> Items { get; } = new List<TEntity>();

        /// <summary>
        /// Gets or sets the total number of content items in the App.
        /// </summary>
        /// <value>
        /// The total number of content items in the App.
        /// </value>
        public long Total { get; set; }
    }
}
