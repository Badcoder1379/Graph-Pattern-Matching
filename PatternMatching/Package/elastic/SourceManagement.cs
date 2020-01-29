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

        public HashSet<Element> GetNodes(Element node, HashSet<Guid> possibleIDs)
        {
            return null;
        }

        public HashSet<Element> GetLinks(Element link, HashSet<Guid> possibleSpurces, HashSet<Guid> possibleTargets)
        {
            return null;
        }

        public int CountNode(Node node)
        {
            var count = client.Count<Node>(c => c.Query(q => q
                .Match(m => m.Field(f => f.ID)
                    .Query(node.ID.ToString())) && q.Match(m => m.Field(f => f.Label).Query(node.Label)))).Count;
            return (int)count;
        }

        public int CountLink(Link link)
        {
            var count = client.Count<Node>(c => c.Query(q => q.Match(m => m.Field(f => f.Label).Query(link.Label)))).Count;
            return (int)count;
        }

        
    }
}
