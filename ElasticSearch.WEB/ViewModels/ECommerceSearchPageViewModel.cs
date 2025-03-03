namespace ElasticSearch.WEB.ViewModels
{
    public class ECommerceSearchPageViewModel
    {
        public long TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public long PageLinkCount { get; set; }
        public List<ECommerceViewModel> List { get; set; } = null!;
        public ECommerceSearchViewModel SearchViewModel { get; set; } = null!;

        public int StartPage()
        {
            return Page - 5 <= 0 ? 1 : Page - 5;
        }

        public long EndPage()
        {
            return Page + 5 >= PageLinkCount ? PageLinkCount : Page + 5;
        }

        public string CreatePageUrl(HttpRequest request, long page, int pageSize)
        {
            string currentUrl = new Uri($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}").AbsoluteUri;

            if (currentUrl.Contains("page", StringComparison.OrdinalIgnoreCase))//aranan ifadenin büyük-küçük harf kontrolünü kapatır.
            {
                currentUrl = currentUrl.Replace($"Page={Page}", $"Page={page}", StringComparison.OrdinalIgnoreCase);
                currentUrl = currentUrl.Replace($"PageSize={PageSize}", $"PageSize={pageSize}", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                currentUrl = $"{currentUrl}?Page={page}";
                currentUrl = $"{currentUrl}&PageSize={pageSize}";
            }

            return currentUrl;
        }
    }
}
