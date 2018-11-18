// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

namespace Squidex.ClientLibrary
{
    public sealed class QueryContext
    {
        public static readonly QueryContext Default = new QueryContext();

        public IEnumerable<string> Languages { get; private set; }

        public bool IsFlatten { get; private set; }

        public bool IsUnpublished { get; private set; }

        private QueryContext()
        {
        }

        public QueryContext Flatten(bool flatten = true)
        {
            return new QueryContext { Languages = Languages, IsUnpublished = IsUnpublished, IsFlatten = flatten };
        }

        public QueryContext WithLanguages(params string[] languages)
        {
            return new QueryContext { Languages = languages, IsUnpublished = IsUnpublished, IsFlatten = IsFlatten };
        }

        public QueryContext Unpublished(bool unpublished = true)
        {
            return new QueryContext { Languages = Languages, IsUnpublished = unpublished, IsFlatten = IsFlatten };
        }
    }
}
