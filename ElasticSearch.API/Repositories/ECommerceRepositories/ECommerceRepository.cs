using System.Collections.Immutable;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.API.Models.ECommerceModel;

namespace ElasticSearch.API.Repositories.ECommerceRepositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "kibana_sample_data_ecommerce";

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<ImmutableList<ECommerce>> TermQueryAsync(string customerFirstName)
        {
            //1. Yol
            //var result = await _client.SearchAsync<ECommerce>(s =>
            //    s.Index(indexName).Query(q => q.Term(t => t.Field("customer_first_name.keyword"!).Value(customerFirstName))));

            //2.Yol
            var termQuery = new TermQuery("customer_first_name.keyword"!)
            {
                Value = customerFirstName, 
                CaseInsensitive = true
            };
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> TermsQueryAsync(List<string> customerFirstName)
        {
            
            List<FieldValue> terms = new List<FieldValue>();
            customerFirstName.ForEach(x =>
            {
                terms.Add(x);
            });

            // 1.Yol
            //var termsQuery = new TermsQuery()
            //{
            //    Field = "customer_first_name.keyword"!,
            //    Terms = new TermsQueryField(terms.AsReadOnly())
            //};
            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termsQuery));

            // 2.Yol
            var result = await _client.SearchAsync<ECommerce>(s => 
                s.Index(indexName)
                    .Size(100)//Tek seferde gelecek veri sayısı
                    .Query(q =>
                q.Terms(t =>
                    t.Field(f => f.CustomerFirstName.Suffix("keyword"))
                        .Terms(new TermsQueryField(terms.AsReadOnly())))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PrefixQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Prefix(p => p.Field(f => f.CustomerFullName.Suffix("keyword")).Value(customerFullName))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> RangeQueryAsync(double fromPrice, double toPrice)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(20).Query(q => q.Range(r => r.NumberRange(nr => nr.Field(f => f.TaxfulTotalPrice).Gte(fromPrice).Lte(toPrice)))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchAllQueryAsync()
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(20).Query(q => q.MatchAll(new MatchAllQuery())));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PaginationQueryAsync(int page, int pageSize)
        {
            //page=1, pageSize=10 => 1-10
            //page=2, pageSize=10 => 11-20
            //page=3, pageSize=10 => 21-30

            int pageFrom = (page-1)*pageSize;

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(pageSize).From(pageFrom).Query(q => q.MatchAll(new MatchAllQuery())));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> WildCardQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Wildcard(w => w.Field(f => f.CustomerFullName.Suffix("keyword")).Wildcard(customerFullName))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> FuzzyQueryAsync(string customerName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Fuzzy(f => f.Field(fi => fi.CustomerFirstName.Suffix("keyword")).Value(customerName).Fuzziness(new Fuzziness(1)))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> OrderingQueryAsync(string customerName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(q => q.Fuzzy(f => f.Field(fi => fi.CustomerFirstName.Suffix("keyword")).Value(customerName).Fuzziness(new Fuzziness(1)))).Sort(so => so.Field(fie => fie.TaxfulTotalPrice, new FieldSort { Order = SortOrder.Desc})));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchQueryFullTextAsync(string categoryName)
        {
            //Aranan kelime or ifadesi ile aranmaktadır.Örnek; Women's Shoes (Women's or Shoes) iki kelimeden biri veya ikisi de olanları getirir.
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(25).Query(q => q.Match(m => m.Field(f => f.Category).Query(categoryName))));

            //Aranan kelime or ifadesi ile aranmaktadır. Bunu and ile arama yapmasını sağlamak için Operator eklememiz gerekmektedir. Örnek; Women's Shoes (Women's and Shoes) olanları getirir.
            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(25).Query(q => q.Match(m => m.Field(f => f.Category).Query(categoryName).Operator(Operator.And))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MultiMatchQueryFullTextAsync(string name)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(25).Query(q => q.MultiMatch(m => 
                m.Fields(new Field("customer_first_name")
                    .And(new Field("customer_last_name"))
                    .And(new Field("customer_full_name")))
                    .Query(name))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchBoolPrefixQueryFullTextAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(25).Query(q => q.MatchBoolPrefix(m => m.Field(f => f.CustomerFullName).Query(customerFullName))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchPhraseQueryFullTextAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(25).Query(q => q.MatchPhrase(m => m.Field(f => f.CustomerFullName).Query(customerFullName))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> CompoundQuery1Async(string cityName, double taxfulTotalPrice, string category, string manufacturer)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(25).Query(q => q.Bool(b => 
                b.Must(m => 
                    m.Term(t => 
                        t.Field("geoip.city_name"!).Value(cityName)))
                .MustNot(mn => 
                    mn.Range(r => 
                        r.NumberRange(nr => 
                            nr.Field(f => f.TaxfulTotalPrice)
                                .Lte(taxfulTotalPrice))))
                .Should(s => 
                    s.Term(te => 
                        te.Field(fi => fi.Category.Suffix("keyword"))
                            .Value(category)))
                .Filter(f => 
                    f.Term(ter => 
                        ter.Field("manufacturer.keyword"!)
                            .Value(manufacturer)))
                )));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> CompoundQuery2Async(string customerFullName)
        {
            // 1. Yol
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(25).Query(q => q.Bool(b =>
                b.Should(s =>
                        s.Match(m =>
                            m.Field(f => f.CustomerFullName)
                                .Query(customerFullName)),
                        s => s.Prefix(p =>
                                p.Field(fi => fi.CustomerFullName.Suffix("keyword"))
                                .Value(customerFullName)))
            )));

            // 2. Yol
            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Size(25).Query(q => q.MatchPhrasePrefix(m => m.Field(f => f.CustomerFullName).Query(customerFullName))));

            foreach (var hit in result.Hits) hit.Source!.Id = hit.Id!;

            return result.Documents.ToImmutableList();
        }
    }
}
