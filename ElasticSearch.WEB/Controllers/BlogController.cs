using ElasticSearch.WEB.Services;
using ElasticSearch.WEB.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.WEB.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogService _blogService;

        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> Search()
        {

            return View(await _blogService.SearchAsync(string.Empty));
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchText)
        {
            ViewBag.SearchText = searchText;

            List<BlogResponseViewModel> blogs = await _blogService.SearchAsync(searchText);
            
            return View(blogs);
        }

        [HttpGet]
        public IActionResult Save()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(BlogCreateViewModel model)
        {
            bool isSuccess = await _blogService.SaveAsync(model);

            if (!isSuccess)
            {
                TempData["result"] = "Kayıt Başarısız!";
                return RedirectToAction(nameof(Save));
            }

            TempData["result"] = "Kayıt Başarılı.";
            return RedirectToAction(nameof(Save));
        }
    }
}
