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

        public List<FixedPattern> MergeNewLinks(Link patternLink, List<Element> condidates, Pattern pattern)
        {
            var valids = new List<Element>();
            foreach(Link condidate in condidates)
            {
                if (fixedElementsMap.Keys.Contains(patternLink.Source))
                {
                    if(fixedElementsMap[patternLink.Source].ID != condidate.Source)
                    {
                        continue;
                    }
                }
                if (fixedElementsMap.Keys.Contains(patternLink.Target))
                {
                    if (fixedElementsMap[patternLink.Target].ID != condidate.Target)
                    {
                        continue;
                    }
                }
                valids.Add(condidate);
            }
            var results = new List<FixedPattern>();
            foreach(var valid in valids)
            {
                var result = new FixedPattern(new Dictionary<Guid, Element>(fixedElementsMap));
                result.fixedElementsMap[patternLink.ID] = valid;
                results.Add(new FixedPattern());
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
