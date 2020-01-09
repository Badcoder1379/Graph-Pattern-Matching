using PatternMatching.Package.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.logic
{
    class FixedPattern
    {

        public bool finished = false;
        public Dictionary<Guid, Element> fixedElementsMap = new Dictionary<Guid, Element>();

        public FixedPattern(Dictionary<Guid, Element> fixedElementsMap)
        {
            this.fixedElementsMap = fixedElementsMap;
        }

        public FixedPattern()
        {
        }

        public List<FixedPattern> MergeNewLinks(Link link, List<Element> ExpandedLinks, Pattern pattern)
        {
            var results = new List<FixedPattern>();
            foreach(Link expandedLink in ExpandedLinks)
            {
                if(fixedElementsMap.ContainsKey(link.Source) && fixedElementsMap[link.Source].ID != expandedLink.Source)
                {
                    continue;
                }
                if(fixedElementsMap.ContainsKey(link.Target) && fixedElementsMap[link.Target].ID != expandedLink.Target)
                {
                    continue;
                }
                var newFixedPattern = new FixedPattern(new Dictionary<Guid, Element>(fixedElementsMap));
                newFixedPattern.fixedElementsMap[link.ID] = expandedLink;
                results.Add(newFixedPattern);
            }
            return results;
        }

        internal FixedPattern MergeNewNodes(Node newExpandedNode,List<Element> condidates, Pattern pattern)
        {
            Guid validId;
            try
            {
                validId = findIdFromNearLinks(newExpandedNode, pattern);
            }
            catch
            {
                return null;
            }
            var result = new FixedPattern(fixedElementsMap);
            var validNodes = condidates.Where(node => node.ID.Equals(validId)).ToList();
            if(validNodes.Count == 0)
            {
                return null;
            }
            if(validNodes.Count > 1)
            {
                throw new Exception("two node hava same guid !!!");
            }
            result.fixedElementsMap[newExpandedNode.ID] = validNodes.First();
            return result;
        }

        private Guid findIdFromNearLinks(Node node, Pattern pattern)
        {
            HashSet<Guid> validIds = new HashSet<Guid>();
            validIds.UnionWith(pattern.Links.Where(x =>
                        fixedElementsMap.ContainsKey(x.ID) &&
                        x.Source == node.ID).Select(link => ((Link)fixedElementsMap[link.ID]).Source));
            validIds.UnionWith(pattern.Links.Where(x =>
                        fixedElementsMap.ContainsKey(x.ID) &&
                        x.Target == node.ID).Select(link => ((Link)fixedElementsMap[link.ID]).Target));
            if (validIds.Count == 0)
            {
                throw new Exception("not valid ID");
            }
            if(validIds.Count > 1)
            {
                throw new Exception("no any valid ID");
            }
            return validIds.First();
        }

        public void Print()
        {
            Console.WriteLine("pattern matched :");
            foreach(var element in fixedElementsMap)
            {
                Console.WriteLine(element.Key.ToString() + "  :  " + element.Value.ID.ToString() + "   as a " + element.Value.Label + " element.");
            }
        }
    }

}
