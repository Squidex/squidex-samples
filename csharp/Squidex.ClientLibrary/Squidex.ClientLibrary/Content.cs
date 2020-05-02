// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary
{
    public abstract class Content<T> : Resource where T : class, new()
    {
        private const string LinkStart = "/api/content/";

        [Obsolete]
        public bool IsPending { get; set; }

        public Guid Id { get; set; }

        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

        public string Status { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public int Version { get; set; }

        public T Data { get; } = new T();

        public string AppName
        {
            get
            {
                return GetDetails().App;
            }
        }

        public string SchemaName
        {
            get
            {
                return GetDetails().Schema;
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

                var hrefParts = href.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                return (hrefParts[0], hrefParts[1]);
            }
            catch
            {
                throw new InvalidOperationException($"Link {self.Href} is malformed.");
            }
        }
    }
}
