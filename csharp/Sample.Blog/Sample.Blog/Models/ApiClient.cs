// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Squidex.ClientLibrary;

namespace Sample.Blog.Models
{
    public class ApiClient : IApiClient
    {
        private readonly SquidexClient<BlogPost, BlogPostData> postsClient;
        private readonly SquidexClient<Page, PageData> pagesClient;

        public ApiClient(IOptions<AppOptions> appOptions)
        {
            var options = appOptions.Value;

            var clientManager =
                new SquidexClientManager(
                    options.Url,
                    options.AppName,
                    options.ClientId,
                    options.ClientSecret);

            pagesClient = clientManager.GetClient<Page, PageData>("pages");
            postsClient = clientManager.GetClient<BlogPost, BlogPostData>("posts");
        }

        public async Task<(long Total, List<BlogPost> Posts)> GetBlogPostsAsync(int page = 0, int pageSize = 3)
        {
            var query = new ODataQuery { Skip = page * pageSize, Top = pageSize };

            var posts = await postsClient.GetAsync(query);

            return (posts.Total, posts.Items);
        }

        public async Task<List<Page>> GetPagesAsync()
        {
            var pages = await pagesClient.GetAsync();

            return pages.Items;
        }

        public async Task<Page> GetPageAsync(string slug)
        {
            var query = new ODataQuery
            {
                Filter = $"data/slug/iv eq '{slug}'"
            };

            var pages = await pagesClient.GetAsync(query);

            return pages.Items.FirstOrDefault();
        }

        public Task<BlogPost> GetBlogPostAsync(string id)
        {
            return postsClient.GetAsync(id);
        }
    }
}
