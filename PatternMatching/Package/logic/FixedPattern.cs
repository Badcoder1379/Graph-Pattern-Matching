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
        public Dictionary<Guid, Element> fixedNodsMap = new Dictionary<Guid, Element>();

        public FixedPattern(Dictionary<Guid, Element> fixedNodsMap)
        {
            this.fixedNodsMap = fixedNodsMap;
        }

        public FixedPattern()
        {
        }

        public List<FixedPattern> MergeNewLinks(Link patternLink, List<Element> condidates, Pattern pattern)
        {
            var valids = new List<Element>();
            foreach(Link condidate in condidates)
            {
                if (fixedNodsMap.Keys.Contains(patternLink.Source))
                {
                    if(fixedNodsMap[patternLink.Source].ID != condidate.Source)
                    {
                        continue;
                    }
                }
                if (fixedNodsMap.Keys.Contains(patternLink.Target))
                {
                    if (fixedNodsMap[patternLink.Target].ID != condidate.Target)
                    {
                        continue;
                    }
                }
                valids.Add(condidate);
            }
            var results = new List<FixedPattern>();
            foreach(var valid in valids)
            {
                var result = new FixedPattern(new Dictionary<Guid, Element>(fixedNodsMap));
                result.fixedNodsMap[patternLink.ID] = valid;
                results.Add(new FixedPattern());
            }
            return results;
        }

        internal FixedPattern MergeNewNodes(Node newExpandedNode, Dictionary<Guid, Element> condidates, Pattern pattern)
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
            var result = new FixedPattern(fixedNodsMap);
            result.fixedNodsMap[validId] = newExpandedNode;
            return result;
        }

        private Guid findIdFromNearLinks(Node node, Pattern pattern)
        {
            HashSet<Guid> validIds = new HashSet<Guid>();
            validIds.UnionWith(pattern.Links.Where(x =>
                        x.Source == node.ID &&
                        fixedNodsMap.ContainsKey(x.Target) &&
                        fixedNodsMap.ContainsKey(x.ID)).Select(x => x.Source));
            validIds.UnionWith(pattern.Links.Where(x =>
                        x.Target == node.ID &&
                        fixedNodsMap.ContainsKey(x.Source) &&
                        fixedNodsMap.ContainsKey(x.ID)).Select(x => x.Target));
            if(validIds.Count == 0)
            {
                throw new Exception("not valid ID");
            }
            if(validIds.Count > 1)
            {
                throw new Exception("no any valid ID");
            }
            return validIds.First();
        }
    }

}
