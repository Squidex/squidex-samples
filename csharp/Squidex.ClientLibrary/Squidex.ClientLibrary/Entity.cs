// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// Base class for all entities.
    /// </summary>
    /// <seealso cref="Resource" />
    public abstract class Entity : Resource
    {
        /// <summary>
        /// Gets or sets the ID of the entity.
        /// </summary>
        /// <value>
        /// The ID of the content.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// The ID of the user or client who created this item.
        /// </summary>
        /// <value>
        /// The ID of the user or client.
        /// </value>
        public string CreatedBy { get; set; }

        /// <summary>
        /// The ID of the user or client modified the content the last time.
        /// </summary>
        /// <value>
        /// The ID of the user or client.
        /// </value>
        public string LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the content has been created.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the content has been modified the last time.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTimeOffset LastModified { get; set; }

        /// <summary>
        /// Gets or sets the version of the entity.
        /// </summary>
        /// <value>
        /// The version as integer. Zero for just created items.
        /// </value>
        public int Version { get; set; }

        /// <summary>
        /// The token that can be used for inline editing.
        /// </summary>
        public string? EditToken { get; set; }
    }
}
