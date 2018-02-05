using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sample.Blog.Models;

namespace Sample.Blog.Controllers
{
    public sealed class HomeController : Controller
    {
        private const int PageSize = 3;
        private readonly IApiClient apiClient;

        public HomeController(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        [Route("/")]
        public async Task<IActionResult> Posts(int page = 0)
        {
            var (total, posts) = await apiClient.GetBlogPostsAsync(page, PageSize);

            var vm = new PostsVM
            {
                Posts = posts,
                Total = total,
                Page = page,
                PageSize = PageSize
            };

            return View(vm);
        }

        [Route("/{slug},{id}/")]
        public async Task<IActionResult> Post(string slug, string id)
        {
            var post = await apiClient.GetBlogPostAsync(id);

            var vm = new PostVM
            {
                Post = post
            };

            return View(vm);
        }

        [Route("/{slug}/")]
        public async Task<IActionResult> Page(string slug)
        {
            var page = await apiClient.GetPageAsync(slug);

            var vm = new PageVM
            {
                Page = page
            };

            return View(vm);
        }

        public IActionResult Error()
        {
            return View(new ErrorVM { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
