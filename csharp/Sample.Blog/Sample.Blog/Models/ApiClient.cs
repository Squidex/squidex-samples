using Microsoft.Extensions.Options;
using Squidex.ClientLibrary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Blog.Models
{
    public class ApiClient : IApiClient
    {
        private readonly SquidexClient<BlogPost, BlogPostData> postsClient;

        public ApiClient(IOptions<AppOptions> appOptions)
        {
            var options = appOptions.Value;

            var clientManager =
                new SquidexClientManager(
                    options.Url,
                    options.AppName,
                    options.ClientId,
                    options.ClientSecret);

            postsClient = clientManager.GetClient<BlogPost, BlogPostData>("posts");
        }

        public Task<BlogPost> GetBlogPostAsync(string id)
        {
            return postsClient.GetAsync(id);
        }

        public async Task<(long Total, List<BlogPost> Posts)> GetBlogPostsAsync(int page = 0, int pageSize = 3)
        {
            var posts = await postsClient.GetAsync(page * pageSize, pageSize);

            return (posts.Total, posts.Items);
        }
    }
}
