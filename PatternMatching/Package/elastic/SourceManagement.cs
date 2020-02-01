using Nest;
using PatternMatching.Package.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.logic
{
    public class SourceManagement
    {
        private ElasticClient client;
        public SourceManagement(ElasticClient client)
        {
            this.client = client;
        }

        public List<Node> GetNodes(Element node, List<Guid> possibleIDs, int pageNumber)
        {
            var labelQuery = new MatchQuery
            {
                Field = "label",
                Query = node.Label
            };

            var idsQuery = new TermsQuery
            {
                Field = "iD.keyword",
                Terms = possibleIDs.Cast<object>().AsEnumerable()
            };

            var query = new BoolQuery()
            {
                Filter = new List<QueryContainer>
                {
                    labelQuery,
                    idsQuery
                }
            };

            var result = client.Search<Node>(x => x
                .Query(q => query)
                .From(pageNumber * Business.PageMax)
                .Size(Business.PageMax)

            );
            return result.Hits.Cast<Node>().ToList();
        }

        public List<Link> GetLinks(Element link, List<Guid> possibleSpurces, List<Guid> possibleTargets, int pageNumber)
        {
            var labelQuery = new MatchQuery
            {
                Field = "label",
                Query = link.Label
            };

            var sourceQuery = new TermsQuery
            {
                Field = "source.keyword",
                Terms = possibleSpurces.Cast<object>().AsEnumerable()
            };

            var targetQuery = new TermsQuery
            {
                Field = "iD.keyword",
                Terms = possibleTargets.Cast<object>().AsEnumerable()
            };



            var query = new BoolQuery()
            {
                Filter = new List<QueryContainer>
                {
                    labelQuery,
                    targetQuery,
                    sourceQuery
                }
            };

            var result = client.Search<Link>(x => x
                .Query(q => query)
                .From(pageNumber * Business.PageMax)
                .Size(Business.PageMax)
            );
            return result.Hits.Cast<Link>().ToList();
        }

        public int CountNode(Element node, List<Guid> possibleIDs)
        {
            var labelQuery = new MatchQuery
            {
                Field = "label",
                Query = node.Label
            };

            var idsQuery = new TermsQuery
            {
                Field = "iD.keyword",
                Terms = possibleIDs.Cast<object>().AsEnumerable()
            };

            var query = new BoolQuery()
            {
                Filter = new List<QueryContainer>
                {
                    labelQuery,
                    idsQuery
                }
            };

            var result = client.Count<Node>(x => x
                .Query(q => query)
            /*
            .Index()
            .From()
            .Size()
            */
            );

            return (int)result.Count;
        }

        public int CountLink(Element link, List<Guid> possibleSpurces, List<Guid> possibleTargets)
        {
            var labelQuery = new MatchQuery
            {
                Field = "label",
                Query = link.Label
            };

            var sourceQuery = new TermsQuery
            {
                Field = "source.keyword",
                Terms = possibleSpurces.Cast<object>().AsEnumerable()
            };

            var targetQuery = new TermsQuery
            {
                Field = "iD.keyword",
                Terms = possibleTargets.Cast<object>().AsEnumerable()
            };



            var query = new BoolQuery()
            {
                Filter = new List<QueryContainer>
                {
                    labelQuery,
                    targetQuery,
                    sourceQuery
                }
            };

            var result = client.Count<Link>(x => x
                .Query(q => query)
            /*
            .Index()
            .From()
            .Size()
            */
            );
            return (int)result.Count;
        }
    }
}
