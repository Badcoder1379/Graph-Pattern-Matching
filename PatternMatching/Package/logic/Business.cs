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
        public static readonly int PageMax = 1;
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
            Algorithm();
            /*
            state = StackState.Adding;
            while (StackPack.Count > 0)
            {
                NextExpand1();
            }
            */
        }


        private void Algorithm()
        {
            while (StackPack.Count > 0)
            {
                while (true)
                {
                    if (lastPack.IsFinished(Pattern))
                    {
                        Results = Results.Union(lastPack.FixedPatterns).ToList();
                        break;
                    }
                    if(lastPack.FixedPatterns.Count == 0)
                    {
                        break;
                    }
                    expandNewElement();
                    addNewPackToStack();
                    state = StackState.Adding;
                }
                while (true)
                {
                    if(lastPack.Page < lastPack.PageCount)
                    {
                        changeLastPagebyNewPage();
                        break;
                    }
                    popLastPackFromStack();
                    if(StackPack.Count == 0)
                    {
                        break;
                    }
                }
            }
        }

        private void NextExpand1()
        {
            if (lastPack.IsFinished(Pattern))
            {
                Results = Results.Union(lastPack.FixedPatterns).ToList();
                if (StackPack.Count == 1 && this.lastPack.Page < this.lastPack.PageCount)
                {
                    changeLastPagebyNewPage();
                }
                else
                {
                    popLastPackFromStack();
                }
                return;
            }
            if (lastPack.FixedPatterns.Count == 0)
            {
                if (lastPack.PageCount > lastPack.Page)
                {
                    changeLastPagebyNewPage();
                    return;
                }
                else
                {
                    popLastPackFromStack();
                    return;
                }
            }

            if (state == StackState.Removing)
            {
                //changing
                if (lastPack.Page < lastPack.PageCount)
                {
                    changeLastPagebyNewPage();
                }
                else
                {
                    //TODO
                }
                return;
            }
            else
            {
                // combinning next page to last page
                if (lastPack.SetsMap[lastPack.LastElementExpanded.ID].Count < PageMax / 2 && lastPack.Page < lastPack.PageCount)
                {
                    combineLastPageAndNewPage();
                    state = StackState.Adding;
                    addNewPackToStack();
                    return;
                }
                //adding next stage
                if (lastPack.expandType == ExpandType.newNode || lastPack.expandType == ExpandType.patternComplitions)
                {
                    var links = Pattern.GetAllNotConpeletedLinks(newPack.FixedElements);
                    if (links.Count > 0)
                    {
                        var link = links.First();
                        expandNewElement(link, -1);
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
            expandNewElement(element, count);
        }

        private void expandNewElement(Element element, int count)
        {
            var pageCount = (count - 1) / PageMax + 1;
            createNewPack(element, 1, pageCount);
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
            var links = lastPack == null ? Pattern.Links : Pattern.GetAllAdjucentLinks(lastPack.FixedElements);
            var nodes = lastPack == null ? Pattern.Nodes : Pattern.GetAllAdjucentNodes(lastPack.FixedElements);
            var dict = new Dictionary<Element, int>();
            foreach (var link in links)
            {
                dict[link] = expander.CountLink(link,
                    lastPack.GetPossibleIds(link.Source),
                    lastPack.GetPossibleIds(link.Target));
            }
            foreach (var node in nodes)
            {
                var possibleIds = (lastPack == null ? null : lastPack.GetPossibleIds(node.ID));
                dict[node] = expander.CountNode(node, possibleIds);
            }
            return dict.OrderByDescending(x => x.Value).First();
        }

        private void createNewPack(Element element, int page, int pageCount)
        {
            newPack = new StackPack();
            newPack.LastElementExpanded = element;
            if (lastPack != null)
            {
                newPack.FixedElements = new List<Guid>(lastPack.FixedElements);
                newPack.SetsMap = new Dictionary<Guid, List<Element>>(lastPack.SetsMap);
                newPack.FixedPatterns = new List<FixedPattern>(lastPack.FixedPatterns);
            }
            else
            {
                newPack.FixedElements = new List<Guid>();
                newPack.SetsMap = new Dictionary<Guid, List<Element>>();
                newPack.FixedPatterns = new List<FixedPattern>();
            }
            newPack.FixedElements.Add(element.ID);
            newPack.Page = page;
            if (pageCount == -1)
            {
                var count = countElement(element);
                newPack.PageCount = (count - 1) / PageMax + 1;
            }
            else
            {
                newPack.PageCount = pageCount;
            }
            ;
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
            state = StackState.Adding;
        }

        private void popLastPackFromStack()
        {
            StackPack.Pop();
            if (StackPack.Count > 0)
            {
                lastPack = StackPack.Peek();
            }
            else
            {
                lastPack = null;
            }
            state = StackState.Removing;
        }



        private void doFirstExpand()
        {
            var minPair = findNextOptimomStage();
            var minElement = minPair.Key;
            var count = minPair.Value;
            expandNewElement(minElement, count);
            addNewPackToStack();
        }



        private void combineLastPageAndNewPage()
        {
            var element = lastPack.LastElementExpanded;
            createNewPack(element, lastPack.Page + 1, lastPack.PageCount);
            var expandedElements = expandLastElementOfNewPack(newPack.Page + 1);
            newPack.SetsMap[element.ID] = expandedElements;
            newPack.expandType = lastPack.expandType;
            newPack.UpdateFixedPatterns(element, Pattern);
            newPack.UpdateSetsFromFixedPatterns();
            newPack.FixedPatterns = newPack.FixedPatterns.Union(lastPack.FixedPatterns).ToList();
            newPack.SetsMap[element.ID] = newPack.SetsMap[element.ID].Union(lastPack.SetsMap[element.ID]).ToList();
        }


        private void changeLastPagebyNewPage()
        {
            var popedPack = StackPack.Peek();
            popLastPackFromStack();

            var element = popedPack.LastElementExpanded;

            createNewPack(element, popedPack.Page + 1, popedPack.PageCount);
            var expandedElements = expandLastElementOfNewPack(newPack.Page);
            newPack.SetsMap[element.ID] = expandedElements;
            newPack.expandType = popedPack.expandType;
            newPack.UpdateFixedPatterns(element, Pattern);
            newPack.UpdateSetsFromFixedPatterns();

            addNewPackToStack();
            state = StackState.Adding;
        }

        public void PrintResults()
        {
            Console.WriteLine(Results.Count);
            foreach (var result in Results)
            {
                result.Print();
            }
        }


    }
}
