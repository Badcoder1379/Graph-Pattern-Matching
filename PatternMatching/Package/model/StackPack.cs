using PatternMatching.Package.logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.model
{
    public class StackPack
    {
        public int Page;
        public int PageCount;
        public Element LastFoundedElement;
        public List<FixedPattern> FixedPatterns = new List<FixedPattern>();
        public ExpandType expandType;
        public Dictionary<Guid, List<Element>> SetsMap = new Dictionary<Guid, List<Element>>();
        public Stack<Guid> FixedElements = new Stack<Guid>();
        public Stack<Guid> expandedElements = new Stack<Guid>();

        public StackPack(int page, int pageCount, Element lastElementExpanded, ExpandType expandType)
        {
            Page = page;
            PageCount = pageCount;
            LastFoundedElement = lastElementExpanded;
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
                SetsMap[elementId] = FixedPatterns.Select(x => x.fixedElementsMap[elementId]).ToList();
            }
        }

        /// <summary>
        /// when you have expanded a new element and you want to update and make new fixed patterns from merge last fixed paterns and new expanded element
        /// </summary>
        /// <param name="newExpandedElement"></param>
        /// <param name="pattern"></param>
        public void UpdateFixedPatterns(Element newExpandedElement, Pattern pattern)
        {
            if (SetsMap.Count > FixedElements.Count + 1)
            {
                var linkId = expandedElements.Pop();
                var nodeId = expandedElements.Peek();
                expandedElements.Push(linkId);
                NormalUpdateFixedPatterns(pattern.GetElementByID(linkId), pattern);
                NormalUpdateFixedPatterns(pattern.GetElementByID(nodeId), pattern);
            }
            else
            {
                if (pattern.IsConnected(this.SetsMap.Keys.ToList()))
                {
                    NormalUpdateFixedPatterns(newExpandedElement, pattern);
                }
            }
        }


        private void NormalUpdateFixedPatterns(Element newExpandedElement, Pattern pattern)
        {
            if (SetsMap.Count == 1 && FixedPatterns.Count == 0)
            {
                foreach (var element in SetsMap[LastFoundedElement.ID])
                {
                    var fixedPattern = new FixedPattern();
                    fixedPattern.fixedElementsMap[LastFoundedElement.ID] = element;
                    FixedPatterns.Add(fixedPattern);
                }
                FixedElements.Push(newExpandedElement.ID);
                return;
            }
            var condidates = SetsMap[newExpandedElement.ID];
            var updatedList = new List<FixedPattern>();
            if (newExpandedElement is Node) //when our new expanded element is a node
            {
                foreach (var fixedPattern in FixedPatterns)
                {
                    var newFixedPatterns = fixedPattern.MergeNewNodes((Node)newExpandedElement, condidates, pattern);
                    if (newFixedPatterns != null)
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
            FixedElements.Push(newExpandedElement.ID);
            FixedPatterns = updatedList;
        }


        public List<Guid> GetPossibleIds(Guid id, Pattern pattern)
        {
            var expandedElements = this.SetsMap.Keys.ToList();

            if (pattern.Links.Select(link => link.ID).Contains(id)) // link
            {
                return SetsMap.ContainsKey(id) ? SetsMap[id].Select(link => link.ID).ToList() : null;
            }
            else // node
            {
                var sets = new List<List<Guid>>();
                foreach(var link in pattern.Links)
                {
                    if(link.Source == id && expandedElements.Contains(link.ID))
                    {
                        sets.Add(SetsMap[link.ID].Select(l => ((Link)l).Source).ToList());
                    }
                }
                foreach (var link in pattern.Links)
                {
                    if (link.Target == id && expandedElements.Contains(link.ID))
                    {
                        sets.Add(SetsMap[link.ID].Select(l => ((Link)l).Target).ToList());
                    }
                }
               
                if (sets.Count == 0)
                {
                    return null;
                }
                else
                {
                    while (sets.Count > 1)
                    {
                        var temp = sets[0];
                        sets.RemoveAt(0);
                        sets[0] = sets[0].Where(x => temp.Contains(x)).ToList();
                    }
                    return sets[0];
                }
            }
        }


        public bool IsFinished(Pattern pattern)
        {
            if (FixedElements.Count == pattern.AllElements.Count)
            {
                return true;
            }
            return false;
        }

    }
}
