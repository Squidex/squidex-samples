using Microsoft.Extensions.Options;
using Squidex.ClientLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var posts = await postsClient.GetAsync(page * pageSize, pageSize);

            return (posts.Total, posts.Items);
        }

        public async Task<List<Page>> GetPagesAsync()
        {
            var pages = await pagesClient.GetAsync();

            return pages.Items;
        }

        public async Task<Page> GetPageAsync(string slug)
        {
            var pages = await pagesClient.GetAsync(filter: $"data/slug/iv eq '{slug}'");

            return pages.Items.FirstOrDefault();
        }

        public Task<BlogPost> GetBlogPostAsync(string id)
        {
            return postsClient.GetAsync(id);
        }
    }
}
