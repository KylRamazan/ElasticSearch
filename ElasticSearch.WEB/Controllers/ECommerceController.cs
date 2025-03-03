using ElasticSearch.WEB.Services;
using ElasticSearch.WEB.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.WEB.Controllers
{
    public class ECommerceController : Controller
    {
        private readonly ECommerceService _eCommerceService;

        public ECommerceController(ECommerceService eCommerceService)
        {
            _eCommerceService = eCommerceService;
        }

        public async Task<IActionResult> Search([FromQuery] ECommerceSearchPageViewModel searchPageViewModel)
        {
            var (eCommerceList, totalCount, pageLinkCount) = await _eCommerceService.SearchAsync(searchPageViewModel.SearchViewModel, searchPageViewModel.Page, searchPageViewModel.PageSize);

            searchPageViewModel.List = eCommerceList;
            searchPageViewModel.TotalCount = totalCount;
            searchPageViewModel.PageLinkCount = pageLinkCount;

            return View(searchPageViewModel);
        }
    }
}
