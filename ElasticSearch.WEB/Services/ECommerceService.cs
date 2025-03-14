﻿using ElasticSearch.WEB.Repositories;
using ElasticSearch.WEB.ViewModels;

namespace ElasticSearch.WEB.Services
{
    public class ECommerceService
    {
        private readonly ECommerceRepository _eCommerceRepository;

        public ECommerceService(ECommerceRepository eCommerceRepository)
        {
            _eCommerceRepository = eCommerceRepository;
        }

        public async Task<(List<ECommerceViewModel>, long totalCount, long pageLinkCount)> SearchAsync(ECommerceSearchViewModel searchViewModel, int page, int pageSize)
        {
            var (eCommerceList, totalCount) = await _eCommerceRepository.SearchAsync(searchViewModel, page, pageSize);

            long pageLinkCountCalculate = totalCount % pageSize;
            long pageLinkCount;

            if (pageLinkCountCalculate == 0)
            {
                pageLinkCount = totalCount / pageSize;
            }
            else
            {
                pageLinkCount = (totalCount / pageSize) + 1;
            }

            var eCommerceListViewModel = eCommerceList.Select(x => new ECommerceViewModel
            {
                Id = x.Id,
                Category = string.Join(",",x.Category),
                CustomerFullName = x.CustomerFullName,
                CustomerFirstName = x.CustomerFirstName,
                CustomerLastName = x.CustomerLastName,
                OrderDate = x.OrderDate.ToShortDateString(),
                Gender = x.Gender.ToLower(),
                OrderId = x.OrderId,
                TaxfulTotalPrice = x.TaxfulTotalPrice
            }).ToList();

            return (eCommerceListViewModel, totalCount, pageLinkCount);
        }
    }
}
