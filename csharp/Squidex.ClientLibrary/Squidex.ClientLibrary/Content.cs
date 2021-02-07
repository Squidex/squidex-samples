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
    /// Represents a content item.
    /// </summary>
    /// <typeparam name="T">The type for the data structure.</typeparam>
    /// <seealso cref="Entity" />
    public abstract class Content<T> : Entity where T : class, new()
    {
        private const string LinkStart = "/api/content/";

        private string status = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this content item is pending.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this content item is pending; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This property is not supported anymore. A content item is pending when the <see cref="NewStatus"/> property is not null.
        /// </remarks>
        [Obsolete]
        public bool IsPending { get; set; }

        /// <summary>
        /// The new status when this content item has an unpublished, new version.
        /// </summary>
        /// <value>
        /// The new status.
        /// </value>
        public string NewStatus { get; set; }

        /// <summary>
        /// Gets the data of the content item.
        /// </summary>
        /// <value>
        /// The data of the content item. Cannot be replaced.
        /// </value>
        public T Data { get; } = new T();

        /// <summary>
        /// Gets the name of the app where this content belongs to.
        /// </summary>
        /// <value>
        /// The name of the app where this content belongs to.
        /// </value>
        public string AppName
        {
            get
            {
                return GetDetails().App;
            }
        }

        /// <summary>
        /// Gets the name of the schema where this content belongs to.
        /// </summary>
        /// <value>
        /// The name of the app schema this content belongs to.
        /// </value>
        public string SchemaName
        {
            get
            {
                return GetDetails().Schema;
            }
        }

        /// <summary>
        /// Gets or sets the status of the content item.
        /// </summary>
        /// <value>
        /// The status of the content item.
        /// </value>
        public string Status
        {
            get
            {
                return !string.IsNullOrEmpty(NewStatus) ? NewStatus : status;
            }
            set
            {
                status = value;
            }
        }

        private (string App, string Schema) GetDetails()
        {
            if (!Links.TryGetValue("self", out var self))
            {
                throw new InvalidOperationException("Content has no self link.");
            }

            try
            {
                var index = self.Href.IndexOf(LinkStart, StringComparison.Ordinal);

                var href = self.Href.Substring(index + LinkStart.Length);

                var hrefParts = href.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                return (hrefParts[0], hrefParts[1]);
            }
            catch
            {
                throw new InvalidOperationException($"Link {self.Href} is malformed.");
            }
        }
    }
}
