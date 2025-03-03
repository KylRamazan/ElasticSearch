
namespace ElasticSearch.WEB.ViewModels
{
    public class BlogResponseViewModel
    {
        public string Id { get; set; } = null!;
        
        public string Title { get; set; } = null!;
        
        public string Content { get; set; } = null!;
        
        public string Tags { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string Created { get; set; } = null!;
    }
}
