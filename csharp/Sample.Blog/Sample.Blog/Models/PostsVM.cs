// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

namespace Sample.Blog.Models
{
    public sealed class PostsVM
    {
        public List<BlogPost> Posts { get; set; }

        public long Total { get; set; }

        public long Page { get; set; }

        public long PageSize { get; set; }
    }
}
