using PatternMatching.Package.logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.model
{
    class StackPack
    {
        public int Page;
        public int PageCount;
        public Element LastElementExpanded;
        public List<FixedPattern> FixedPatterns = new List<FixedPattern>();
        public ExpandType expandType;
        public Dictionary<Guid, List<Element>> SetsMap = new Dictionary<Guid, List<Element>>();
        public List<Guid> FixedElements = new List<Guid>();

        public StackPack(int page, int pageCount, Element lastElementExpanded, ExpandType expandType)
        {
            Page = page;
            PageCount = pageCount;
            LastElementExpanded = lastElementExpanded;
            this.expandType = expandType;
        }

        public StackPack() { }

        /// <summary>
        /// when have updated your fixed patterns and some last fixed patterns are deleted, you cau use this to decress sets size and optimom them
        /// </summary>
        public void UpdateSetsFromFixedPatterns()
        {
            foreach (var elementId in FixedElements)
            {
                SetsMap[elementId] = FixedPatterns.Select(x => x.fixedNodsMap[elementId]).ToList();
            }
        }

        /// <summary>
        /// when you have expanded a new element and you want to update and make new fixed patterns from merge last fixed paterns and new expanded element
        /// </summary>
        /// <param name="newExpandedElement"></param>
        /// <param name="pattern"></param>
        public void UpdateFixedPatterns(Element newExpandedElement, Pattern pattern)
        {
            var condidates = SetsMap[newExpandedElement.ID];
            var updatedList = new List<FixedPattern>();
            if (newExpandedElement is Node) //when our new expanded element is a node
            {
                var dict = new Dictionary<Guid, Element>();
                foreach(var condidate in condidates)
                {
                    dict[condidate.ID] = condidate;
                }
                foreach(var fixedPattern in FixedPatterns)
                {
                    var newFixedPatterns = fixedPattern.MergeNewNodes((Node)newExpandedElement, dict, pattern);
                    if(newFixedPatterns != null)
                    {
                        updatedList.Add(newFixedPatterns);
                    }
                }
            }
            else                            // when our new expanded is a link
            {
                foreach (var fixedPattern in FixedPatterns)
                {
                    var newFixedPatterns = fixedPattern.MergeNewLinks((Link)newExpandedElement, condidates, pattern);
                    updatedList = updatedList.Union(newFixedPatterns).ToList();
                }
            }
            FixedPatterns = updatedList;
        }


        public List<Guid> GetPossibleIds(Guid id)
        {
            return SetsMap[id].Select(x => x.ID).ToList();
        }


        public bool IsFinished(Pattern pattern)
        {
            if(FixedElements.Count == pattern.Nodes.Count)
            {
                return true;
            }
            return false;
        }

    }
}
