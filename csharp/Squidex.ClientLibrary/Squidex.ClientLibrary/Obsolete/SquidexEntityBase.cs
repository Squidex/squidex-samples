// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary
{
    [Obsolete]
    public abstract class SquidexEntityBase<T> : EntityBase where T : class, new()
    {
        private const string LinkStart = "/api/content/";

        public T Data { get; } = new T();

        public T DataDraft { get; set; }

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
