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
