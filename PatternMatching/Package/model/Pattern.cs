using PatternMatching.Package.logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.model
{
    public class Pattern
    {
        public List<Node> Nodes;
        public List<Link> Links;
        public List<Element> AllElements;

        public Pattern(Graph graph)
            : this(graph.Nodes, graph.Links) { }


        public Pattern(List<Node> nodes, List<Link> links)
        {
            this.Nodes = nodes;
            this.Links = links;
            this.AllElements = new List<Element>().Union(Links).Union(Nodes).ToList();
        }

        public List<Link> FindNotCompeletedLinks(List<Guid> fixedIds)
        {
            var nodeIDs = Nodes.Select(x => x.ID);
            var iteratorNodes = fixedIds.Where(x => nodeIDs.Contains(x));
            return Links.Where(x => iteratorNodes.Contains(x.Source) &&
                                    iteratorNodes.Contains(x.Target) &&
                                    !fixedIds.Contains(x.ID)).ToList();
        }


        public List<Link> GetAllAdjucentLinks(List<Guid> fixedElements)
        {
            List<Guid> resultList = new List<Guid>();
            foreach (Link link in Links.Where(x => !fixedElements.Contains(x.ID)))
            {
                if (fixedElements.Contains(link.Source) || fixedElements.Contains(link.Target))
                {
                    resultList.Add(link.ID);
                }
            }
            return Links.Where(x => resultList.Contains(x.ID) && !fixedElements.Contains(x.ID)).ToList();
        }

        public List<Node> GetAllAdjucentNodes(List<Guid> fixedElements)
        {
            List<Guid> resultList = new List<Guid>();
            foreach (Link link in Links.Where(link => fixedElements.Contains(link.ID)))
            {
                if (!fixedElements.Contains(link.Target))
                {
                    resultList.Add(link.Target);
                }
                if (!fixedElements.Contains(link.Source))
                {
                    resultList.Add(link.Source);
                }

            }

            foreach (var link in Links)
            {
                if (fixedElements.Contains(link.Source) && !fixedElements.Contains(link.Target))
                {
                    resultList.Add(link.Target);
                }
                if (!fixedElements.Contains(link.Source) && fixedElements.Contains(link.Target))
                {
                    resultList.Add(link.Source);
                }
            }

            return Nodes.Where(x => resultList.Contains(x.ID) && !fixedElements.Contains(x.ID)).ToList();
        }

        public List<Link> GetAllNotConpeletedLinks(List<Guid> fixedElements)
        {
            return Links.Where(x => fixedElements.Contains(x.Source) && fixedElements.Contains(x.Target) && !fixedElements.Contains(x.ID)).ToList();
        }

        public List<Link> GetAllAdjucentLinks(Guid NodeId)
        {
            return Links.Where(x => x.Target == NodeId || x.Source == NodeId).ToList();
        }

        public bool IsConnected(List<Guid> expandedElements)
        {
            var dict = new Dictionary<Guid, int>();
            int i = 0;
            foreach (var node in Nodes.Where(node => expandedElements.Contains(node.ID)))
            {
                dict[node.ID] = i;
                i++;
            }

            for (int j = 0; j < dict.Count; j++)
            {
                foreach (var link in Links.Where(link => expandedElements.Contains(link.ID) &&
                                                    expandedElements.Contains(link.Source) &&
                                                    expandedElements.Contains(link.Target)))
                {
                    var higherNode = link.Source;
                    var lowerNode = link.Source;
                    if (dict[link.Target] > dict[link.Source])
                    {
                        higherNode = link.Target;
                        lowerNode = link.Source;
                    }
                    dict[higherNode] = dict[lowerNode];
                }
            }
            int count = new HashSet<int>(dict.Values).Count;
            return count < 2;
        }


        public Element GetElementByID(Guid id)
        {
            return AllElements.Where(element => element.ID == id).First();
        }

    }
}

