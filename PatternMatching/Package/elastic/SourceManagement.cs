﻿using Nest;
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
            var filter = new List<QueryContainer>();

            var labelQuery = new MatchQuery
            {
                Field = "label",
                Query = node.Label
            };
            filter.Add(labelQuery);

            
            if (possibleIDs != null)
            {
                var idsQuery = new TermsQuery
                {
                    Field = "iD.keyword",
                    Terms = possibleIDs.Cast<object>().AsEnumerable()
                };
                filter.Add(idsQuery);
            }

            var query = new BoolQuery()
            {
                Filter = filter
            };

            var result = client.Search<Node>(x => x
                .Query(q => query)
                .From((pageNumber-1) * Business.PageMax)
                .Size(Business.PageMax)

            );
            return result.Hits.Select(x => x.Source).ToList();
        }

        public List<Link> GetLinks(Element link, List<Guid> possibleSpurces, List<Guid> possibleTargets, int pageNumber)
        {
            var filter = new List<QueryContainer>();
            var labelQuery = new MatchQuery
            {
                Field = "label",
                Query = link.Label
            };
            filter.Add(labelQuery);

            
            if (possibleSpurces != null)
            {
                var sourceQuery = new TermsQuery
                {
                    Field = "source.keyword",
                    Terms = possibleSpurces.Cast<object>().AsEnumerable()
                };
                filter.Add(sourceQuery);
            }

            
            if (possibleTargets != null)
            {
                var targetQuery = new TermsQuery
                {
                    Field = "target.keyword",
                    Terms = possibleTargets.Cast<object>().AsEnumerable()
                };
                filter.Add(targetQuery);
            }


            var query = new BoolQuery()
            {
                Filter = filter
            };

            var result = client.Search<Link>(x => x
                .Query(q => query)
                .From((pageNumber-1) * Business.PageMax)
                .Size(Business.PageMax)
            );
            return result.Hits.Select(x => x.Source).ToList();
        }

        public int CountNode(Element node, List<Guid> possibleIDs)
        {
            var filter = new List<QueryContainer>();

            var labelQuery = new MatchQuery
            {
                Field = "label",
                Query = node.Label
            };
            filter.Add(labelQuery);

            
            if(possibleIDs != null)
            {
                var idsQuery = new TermsQuery
                {
                    Field = "iD.keyword",
                    Terms = possibleIDs.Cast<object>().AsEnumerable()
                };
                filter.Add(idsQuery);
            }

            var query = new BoolQuery()
            {
                Filter = filter
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
            var filter = new List<QueryContainer>();
            var labelQuery = new MatchQuery
            {
                Field = "label",
                Query = link.Label
            };
            filter.Add(labelQuery);

            
            if(possibleSpurces != null)
            {
                var sourceQuery = new TermsQuery
                {
                    Field = "source.keyword",
                    Terms = possibleSpurces.Cast<object>().AsEnumerable()
                };
                filter.Add(sourceQuery);
            }

            
            if(possibleTargets != null)
            {
                var targetQuery = new TermsQuery
                {
                    Field = "target.keyword",
                    Terms = possibleTargets.Cast<object>().AsEnumerable()
                };
                filter.Add(targetQuery);
            }


            var query = new BoolQuery()
            {
                Filter = filter
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
