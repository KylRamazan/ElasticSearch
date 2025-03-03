using ElasticSearch.API.Repositories.ECommerceRepositories;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ECommerceController : ControllerBase
    {
        private readonly ECommerceRepository _commerceRepository;

        public ECommerceController(ECommerceRepository commerceRepository)
        {
            _commerceRepository = commerceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName)
        {
            return Ok(await _commerceRepository.TermQueryAsync(customerFirstName));
        }

        [HttpGet]
        public async Task<IActionResult> TermsQuery(List<string> customerFirstName)
        {
            return Ok(await _commerceRepository.TermsQueryAsync(customerFirstName));
        }

        [HttpGet]
        public async Task<IActionResult> PrefixQuery(string customerFullName)
        {
            return Ok(await _commerceRepository.PrefixQueryAsync(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> RangeQuery(double fromPrice, double toPrice)
        {
            return Ok(await _commerceRepository.RangeQueryAsync(fromPrice, toPrice));
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllQuery()
        {
            return Ok(await _commerceRepository.MatchAllQueryAsync());
        }

        [HttpGet]
        public async Task<IActionResult> PaginationQuery(int page = 1, int pageSize = 10)
        {
            return Ok(await _commerceRepository.PaginationQueryAsync(page, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> WildCardQuery(string customerFullName)
        {
            return Ok(await _commerceRepository.WildCardQueryAsync(customerFullName));
        }
        
        [HttpGet]
        public async Task<IActionResult> FuzzyQuery(string customerName)
        {
            return Ok(await _commerceRepository.FuzzyQueryAsync(customerName));
        }

        [HttpGet]
        public async Task<IActionResult> OrderingQuery(string customerName)
        {
            return Ok(await _commerceRepository.OrderingQueryAsync(customerName));
        }
        
        [HttpGet]
        public async Task<IActionResult> MatchQueryFullText(string categoryName)
        {
            return Ok(await _commerceRepository.MatchQueryFullTextAsync(categoryName));
        }

        [HttpGet]
        public async Task<IActionResult> MultiMatchQueryFullText(string name)
        {
            return Ok(await _commerceRepository.MultiMatchQueryFullTextAsync(name));
        }

        [HttpGet]
        public async Task<IActionResult> MatchBoolPrefixQueryFullText(string customerFullName)
        {
            return Ok(await _commerceRepository.MatchBoolPrefixQueryFullTextAsync(customerFullName));
        }
        
        [HttpGet]
        public async Task<IActionResult> MatchPhraseQueryFullText(string customerFullName)
        {
            return Ok(await _commerceRepository.MatchPhraseQueryFullTextAsync(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> CompoundQuery1(string cityName, double taxfulTotalPrice, string category, string manufacturer)
        {
            return Ok(await _commerceRepository.CompoundQuery1Async(cityName, taxfulTotalPrice, category, manufacturer));
        }

        [HttpGet]
        public async Task<IActionResult> CompoundQuery2(string customerFullName)
        {
            return Ok(await _commerceRepository.CompoundQuery2Async(customerFullName));
        }
    }
}
