using PatternMatching.Package.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.logic
{
    class Business
    {
        public Pattern Pattern;
        public Expander expander;
        public Stack<StackPack> StackPack = new Stack<StackPack>();
        private StackPack lastPack;
        private StackPack newPack;
        public static readonly int PageMax = 7;
        public List<FixedPattern> Results = new List<FixedPattern>();
        private StackState state = StackState.Removing;

        public Business(Pattern pattern, Expander expander)
        {
            this.expander = expander;
            this.Pattern = pattern;
        }

        public void Run()
        {
            doFirstExpand();
            state = StackState.Adding;
            while (StackPack.Count > 0)
            {
                nextExpand();
            }
        }

        private void nextExpand()
        {

            var last = StackPack.Peek();
            if (last.IsFinished(Pattern))
            {
                Results = Results.Union(last.FixedPatterns).ToList();
                StackPack.Pop();
                state = StackState.Removing;
                return;
            }
            if (last.FixedPatterns.Count == 0)
            {
                StackPack.Pop();
                state = StackState.Removing;
                return;
            }
            if (state == StackState.Removing)
            {
                //changing
                if(last.Page < last.PageCount)
                {
                    var element = last.LastElementExpanded;
                    createNewPack(element, last.Page + 1, last.PageCount);
                    var expandedElements = expandLastElementOfNewPack(newPack.Page + 1);
                    newPack.SetsMap[element.ID] = expandedElements;
                    newPack.expandType = last.expandType;
                    newPack.UpdateFixedPatterns(element, Pattern);
                    newPack.UpdateSetsFromFixedPatterns();
                    state = StackState.Adding;
                    addNewPackToStack();
                    return;
                }
                StackPack.Pop();
                state = StackState.Removing;
                return;
            }
            else
            {
                // check for finish
                if (last.IsFinished(Pattern))
                {
                    Results = Results.Union(last.FixedPatterns).ToList();
                    StackPack.Pop();
                    state = StackState.Removing;
                    addNewPackToStack();
                    return;
                }
                // combinning next page to last page
                if (last.SetsMap[last.LastElementExpanded.ID].Count < PageMax / 2 && last.Page < last.PageCount)
                {
                    var element = last.LastElementExpanded;
                    createNewPack(element, last.Page + 1, last.PageCount);
                    var expandedElements = expandLastElementOfNewPack(newPack.Page + 1);
                    newPack.SetsMap[element.ID] = expandedElements;
                    newPack.expandType = last.expandType;
                    newPack.UpdateFixedPatterns(element, Pattern);
                    newPack.UpdateSetsFromFixedPatterns();
                    newPack.FixedPatterns = newPack.FixedPatterns.Union(last.FixedPatterns).ToList();
                    newPack.SetsMap[element.ID] = newPack.SetsMap[element.ID].Union(last.SetsMap[element.ID]).ToList();
                    state = StackState.Adding;
                    addNewPackToStack();
                    return;
                }
                //adding next stage
                if (last.expandType == ExpandType.newNode || last.expandType == ExpandType.patternComplitions)
                {
                    var links = Pattern.GetAllNotConpeletedLinks(newPack.FixedElements);
                    if (links.Count > 0)
                    {
                        var link = links.First();
                        createNewPack(link, 1, -1);
                        var expandedLinks = expandLastElementOfNewPack(newPack.Page + 1);
                        newPack.SetsMap[link.ID] = expandedLinks;
                        newPack.expandType = last.expandType;
                        newPack.UpdateFixedPatterns(link, Pattern);
                        newPack.UpdateSetsFromFixedPatterns();
                        state = StackState.Adding;
                        addNewPackToStack();
                        return;
                    }
                }
                expandNewElement();
                addNewPackToStack();
                state = StackState.Adding;
            }

        }

        private void expandNewElement()
        {
            var minPair = findNextOptimomStage();
            var element = minPair.Key;
            var count = minPair.Value;
            createNewPack(element, 1, count);
            var expandedElements = expandLastElementOfNewPack(newPack.Page);
            newPack.SetsMap[element.ID] = expandedElements;
            if (element is Node)
            {
                newPack.expandType = ExpandType.newNode;
            }
            else
            {
                newPack.expandType = ExpandType.newLink;
            }
            newPack.UpdateFixedPatterns(element, Pattern);
            newPack.UpdateSetsFromFixedPatterns();
        }

        private List<Element> expandLastElementOfNewPack(int page)
        {
            var element = newPack.LastElementExpanded;
            if (element is Node)
            {
                return expander.ExpandNode((Node)element, newPack.GetPossibleIds(element.ID), page).ToList();
            }
            else
            {
                var link = (Link)element;
                return expander.ExpandLink(link,
                    newPack.GetPossibleIds(link.Source),
                    newPack.GetPossibleIds(link.Target),
                    page);
            }
        }

        private KeyValuePair<Element, int> findNextOptimomStage()
        {
            var links = Pattern.GetAllAdjucentLinks(lastPack.FixedElements);
            var nodes = Pattern.GetAllAdjucentNodes(lastPack.FixedElements);
            var dict = new Dictionary<Element, int>();
            foreach (var link in links)
            {
                dict[link] = expander.CountLink(link,
                    lastPack.GetPossibleIds(link.Source),
                    lastPack.GetPossibleIds(link.Target));
            }
            foreach (var node in nodes)
            {
                dict[node] = expander.CountNode(node, lastPack.GetPossibleIds(node.ID));
            }
            return dict.OrderByDescending(x => x.Value).First();
        }

        private void createNewPack(Element element, int page, int elementCount)
        {
            if (elementCount == -1)
            {
                elementCount = countElement(element);
            }
            var pageCount = (elementCount - 1) / PageMax + 1;
            newPack = new StackPack();
            newPack.LastElementExpanded = element;
            newPack.FixedElements = new List<Guid>(lastPack.FixedElements);
            newPack.FixedElements.Add(element.ID);
            newPack.SetsMap = new Dictionary<Guid, List<Element>>(lastPack.SetsMap);
            newPack.FixedPatterns = lastPack.FixedPatterns;
            newPack.Page = page;
            newPack.PageCount = pageCount;
        }

        private int countElement(Element element)
        {
            if (element is Node)
            {
                return expander.CountNode((Node)element, newPack.GetPossibleIds(element.ID));
            }
            else
            {
                var link = (Link)element;
                return expander.CountLink(link,
                    newPack.GetPossibleIds(link.Source),
                    newPack.GetPossibleIds(link.Target));
            }
        }

        private void addNewPackToStack()
        {
            StackPack.Push(newPack);
            lastPack = newPack;
        }



        private void doFirstExpand()
        {
            var links = Pattern.Links;
            var nodes = Pattern.Nodes;
            var dict = new Dictionary<Element, int>();
            foreach (var link in links)
            {
                dict[link] = expander.CountLink(link, null, null);
            }
            foreach (var node in nodes)
            {
                dict[node] = expander.CountNode(node, null);
            }
            var minElement = dict.OrderByDescending(x => x.Value).First().Key;
            var count = dict[minElement];

            newPack = new StackPack();
            newPack.LastElementExpanded = minElement;
            newPack.FixedElements = new List<Guid>();
            newPack.FixedElements.Add(minElement.ID);
            newPack.SetsMap = new Dictionary<Guid, List<Element>>();
            newPack.FixedPatterns = new List<FixedPattern>();
            newPack.Page = 1;
            newPack.PageCount = (count - 1) / PageMax + 1;


            var expandedElements = expandLastElementOfNewPack(newPack.Page);
            newPack.SetsMap[minElement.ID] = expandedElements;
            if (minElement is Node)
            {
                newPack.expandType = ExpandType.newNode;
            }
            else
            {
                newPack.expandType = ExpandType.newLink;
            }
            foreach (var element in newPack.SetsMap[minElement.ID])
            {
                var fixedPattern = new FixedPattern();
                fixedPattern.fixedElementsMap[minElement.ID] = element;
                newPack.FixedPatterns.Add(fixedPattern);
            }
            addNewPackToStack();
        }

        public void PrintResults()
        {
            foreach (var result in Results)
            {
                result.Print();
            }
        }
    }
}



















/*
private void newNode()
{
    expandNewNextNode();
    newPack.UpdateFixedPatterns(newPack.LastElementExpanded, Pattern);
    newPack.UpdateSetsFromFixedPatterns();
    addNewPackToStack();
}

private void newLink(Link foundedLink)
{
    expandNewNextLink();
    addNewPackToStack();
    newPack.UpdateFixedPatterns(foundedLink, Pattern);
    newPack.UpdateSetsFromFixedPatterns();
    addNewPackToStack();
    var notCompletedNodeId = foundedLink.Source;
    if (newPack.FixedElements.Contains(foundedLink.Source))
    {
        notCompletedNodeId = foundedLink.Target;
    }
    var notCompletedNode = Pattern.Nodes.Where(x => x.ID == notCompletedNodeId).First();
    createNewPack(notCompletedNode);
    var count = expander.CountNode(notCompletedNode, newPack.GetPossibleIds(notCompletedNodeId));
    newPack.PageCount = count / PageCount;
    newPack.Page = 1;
    expandNewNextNode();
    newPack.UpdateFixedPatterns(notCompletedNode, Pattern);
    newPack.UpdateSetsFromFixedPatterns();
    addNewPackToStack();
}

private void expandNewNextNode()
{
    var newNode = newPack.LastElementExpanded;
    var expandedNodes = expander.ExpandNode((Node)newNode, lastPack.GetPossibleIds(newNode.ID));
    newPack.SetsMap[newNode.ID] = expandedNodes.ToList();
}

private void expandNewNextLink()
{
    var newLink = (Link)newPack.LastElementExpanded;
    var expandedLinks = expander.ExpandLink(newLink,
                    lastPack.GetPossibleIds(newLink.Source),
                    lastPack.GetPossibleIds(newLink.Target));
}

private void expandNotCompeletedLink(Link link)
{
    var expandedLinks = expander.ExpandLink(link,
        lastPack.GetPossibleIds(link.Source),
        lastPack.GetPossibleIds(link.Target));
    createNewPack(link);
    newPack.UpdateFixedPatterns(newPack.LastElementExpanded, Pattern);
    newPack.UpdateSetsFromFixedPatterns();
}



private void expandNotCompeleted()
{
    var notExpandedLinks = Pattern.GetAllNotConpeletedLinks(lastPack.FixedElements);
    foreach (var link in notExpandedLinks)
    {
        expandNotCompeletedLink(link);
    }
}




}
*/

