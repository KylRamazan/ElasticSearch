using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.WEB.Models;
using ElasticSearch.WEB.ViewModels;

namespace ElasticSearch.WEB.Repositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<(List<ECommerce> list, long count)> SearchAsync(ECommerceSearchViewModel searchViewModel, int page, int pageSize)
        {
            List<Action<QueryDescriptor<ECommerce>>> listQuery = new();

            if (searchViewModel == null)
            {
                listQuery.Add(q => q.MatchAll(new MatchAllQuery()));

                return await CalculateResultSet(page, pageSize, listQuery);
            }

            if (!string.IsNullOrEmpty(searchViewModel.Category))
            {
                listQuery.Add(q => q.Match(m => m.Field(f => f.Category).Query(searchViewModel.Category)));
            }

            if (!string.IsNullOrEmpty(searchViewModel.CustomerFullName))
            {
                listQuery.Add(q => q.Match(m => m.Field(f => f.CustomerFullName).Query(searchViewModel.CustomerFullName)));
            }

            if (searchViewModel.OrderDateStart.HasValue)
            {
                listQuery.Add(q => q.Range(r => r.DateRange(dt => dt.Field(f => f.OrderDate).Gte(searchViewModel.OrderDateStart.Value))));
            }

            if (searchViewModel.OrderDateEnd.HasValue)
            {
                listQuery.Add(q => q.Range(r => r.DateRange(dt => dt.Field(f => f.OrderDate).Lte(searchViewModel.OrderDateEnd.Value))));
            }

            if (!string.IsNullOrEmpty(searchViewModel.Gender))
            {
                listQuery.Add(q => q.Term(t => t.Field(f => f.Gender).Value(searchViewModel.Gender).CaseInsensitive()));
            }

            if (!listQuery.Any())
            {
                listQuery.Add(q => q.Match(m => m.Field(f => f.Category).Query(searchViewModel.Category)));
            }

            return await CalculateResultSet(page, pageSize, listQuery);
        }

        private async Task<(List<ECommerce> list, long count)> CalculateResultSet(int page, int pageSize, List<Action<QueryDescriptor<ECommerce>>> listQuery)
        {
            int pageFrom = (page - 1) * pageSize;

            var response = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Size(pageSize).From(pageFrom)
                .Query(q =>
                    q.Bool(b =>
                        b.Must(listQuery.ToArray())
                    )));

            foreach (var hit in response.Hits) hit.Source!.Id = hit.Id!;

            return (list: response.Documents.ToList(), response.Total);
        }
    }
}
