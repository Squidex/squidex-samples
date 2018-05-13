// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Sample.Blog.Models
{
    public sealed class AppOptions
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AppName { get; set; }

        public string Url { get; set; }
    }
}
