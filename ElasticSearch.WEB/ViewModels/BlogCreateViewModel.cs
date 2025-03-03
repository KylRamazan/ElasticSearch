
using System.ComponentModel.DataAnnotations;

namespace ElasticSearch.WEB.ViewModels
{
    public class BlogCreateViewModel
    {
        [Display(Name = "Blog Title")]
        [Required]
        public string Title { get; set; } = null!;

        [Display(Name = "Blog Content")]
        [Required]
        public string Content { get; set; } = null!;

        [Display(Name = "Blog Tags")]
        [Required]
        public string Tags { get; set; } = null!;
    }
}
