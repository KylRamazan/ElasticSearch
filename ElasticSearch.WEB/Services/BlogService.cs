using ElasticSearch.WEB.Models;
using ElasticSearch.WEB.Repositories;
using ElasticSearch.WEB.ViewModels;

namespace ElasticSearch.WEB.Services
{
    public class BlogService
    {
        private readonly BlogRepository _blogRepository;

        public BlogService(BlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<bool> SaveAsync(BlogCreateViewModel model)
        {
            Blog newBlog = new Blog
            {
                Title = model.Title,
                Content = model.Content,
                Tags = model.Tags.Split(", ").ToList(),
                UserId = Guid.NewGuid()
            };

            Blog? responseBlog = await _blogRepository.SaveAsync(newBlog);

            return responseBlog != null;
        }

        public async Task<List<BlogResponseViewModel>> SearchAsync(string searchText)
        {
            List<Blog> responseBlogs = await _blogRepository.SearchAsync(searchText);
            return responseBlogs.Select(x => new BlogResponseViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                Tags = string.Join(",", x.Tags),
                UserId = x.UserId.ToString(),
                Created = x.Created.ToShortDateString()
            }).ToList();
        }
    }
}
