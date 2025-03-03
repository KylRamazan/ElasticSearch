using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearch.WEB.Models;

namespace ElasticSearch.WEB.Repositories
{
    public class BlogRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName = "blog";

        public BlogRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<Blog?> SaveAsync(Blog newBlog)
        {
            newBlog.Created = DateTime.Now;

            var response = await _client.IndexAsync(newBlog, i => i.Index(indexName));

            if (!response.IsValidResponse) return null;

            newBlog.Id = response.Id;
            return newBlog;
        }

        public async Task<List<Blog>> SearchAsync(string searchText)
        {
            // 1.Yol
            //var response = await _client.SearchAsync<Blog>(s => s.Index(indexName)
            //    .Size(20)
            //    .Query(q =>
            //        q.Bool(b =>
            //            b.Should(so =>
            //                    so.MatchBoolPrefix(m =>
            //                        m.Field(f => f.Title).Query(searchText)),
            //                so => so.Match(ma =>
            //                    ma.Field(fi => fi.Content).Query(searchText)))
            //        )));


            // 2.Yol
            List<Action<QueryDescriptor<Blog>>> listQuery = new();

            Action<QueryDescriptor<Blog>> matchAll = q => q.MatchAll(new MatchAllQuery());

            Action<QueryDescriptor<Blog>> matchContent = q => q.Match(m => m.Field(f => f.Content).Query(searchText));

            Action<QueryDescriptor<Blog>> matchBoolPrefixTitle = q => q.MatchBoolPrefix(m => m.Field(f => f.Title).Query(searchText));

            Action<QueryDescriptor<Blog>> termTag = q => q.Term(t => t.Field(f => f.Tags).Value(searchText));

            if (string.IsNullOrEmpty(searchText))
            {
                listQuery.Add(matchAll);
            }
            else
            {
                listQuery.Add(matchContent);
                listQuery.Add(matchBoolPrefixTitle);
                listQuery.Add(termTag);
            }

            var response = await _client.SearchAsync<Blog>(s => s.Index(indexName)
                .Size(20)
                .Query(q => 
                    q.Bool(b => 
                        b.Should(listQuery.ToArray())
                    )));

            foreach (var hit in response.Hits) hit.Source!.Id = hit.Id!;

            return response.Documents.ToList();
        }
    }
}
