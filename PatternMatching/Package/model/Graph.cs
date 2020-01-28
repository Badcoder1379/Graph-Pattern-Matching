using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.model
{
    public class Graph
    {
        public List<Node> Nodes;
        public List<Link> Links;

        public Graph(List<Node> nodes, List<Link> links)
        {
            Nodes = nodes;
            Links = links;
        }
    }
}
