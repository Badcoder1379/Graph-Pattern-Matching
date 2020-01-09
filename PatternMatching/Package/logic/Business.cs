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

        public Business(Pattern pattern, Expander expander)
        {
            this.expander = expander;
            this.Pattern = pattern;
        }

        public void Run()
        {
           // ExpandFirstTimeAndFirstPack();
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
            ExpandElementOfMinCount();
            if(newPack == null)
            {
                return;
            }
            while (StackPack.Count > 0)
            {
                while (true)
                {
                    if (lastPack.IsFinished(Pattern))
                    {
                        Results = Results.Union(lastPack.FixedPatterns).ToList();
                        break;
                    }
                    if (lastPack.FixedPatterns.Count == 0)
                    {
                        break;
                    }
                    
                    if(lastPack.SetsMap[lastPack.LastElementExpanded.ID].Count < PageMax / 2 && lastPack.Page < lastPack.PageCount)
                    {
                        CombineLastPackWithANewPack();
                        continue;
                    }
                    
                    AddingExpand();
                }
                while (true)
                {
                    if (lastPack.Page < lastPack.PageCount)
                    {
                        ChangeLastPagebyNextPage();
                        break;
                    }
                    PopLastPackFromStack();
                    if (StackPack.Count == 0)
                    {
                        break;
                    }
                }
            }
        }

        private void AddingExpand()
        {
            var links = Pattern.GetAllNotConpeletedLinks(newPack.FixedElements);
            if (links.Count > 0)
            {
                var link = links.First();
                ExpandElement(link, -1);
                return;
            }

            ExpandElementOfMinCount();
        }



        private void ExpandElementOfMinCount()
        {
            var minPair = FindOptimomAndMinimomStage();
            var element = minPair.Key;
            var count = minPair.Value;
            if(count > 0)
            {
                ExpandElement(element, count);
            }
            else
            {
                if (newPack != null)
                {
                    newPack.FixedPatterns = new List<FixedPattern>();
                }
            }
        }

        private void ExpandElement(Element element, int count)
        {
            var pageCount = (count - 1) / PageMax + 1;
            CreateNewPack(element, 1, pageCount);
            var expandedElements = ExpandLastElementOfNewPack(newPack.Page);
            newPack.SetsMap[element.ID] = expandedElements;
            newPack.FixedElements.Add(element.ID);
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
            AddNewPackToStack();
        }

        private List<Element> ExpandLastElementOfNewPack(int page)
        {
            var element = newPack.LastElementExpanded;
            if (element is Node)
            {
                return expander.ExpandNode((Node)element, newPack.GetPossibleIds(element.ID, Pattern), page).ToList();
            }
            else
            {
                var link = (Link)element;
                return expander.ExpandLink(link,
                    newPack.GetPossibleIds(link.Source, Pattern),
                    newPack.GetPossibleIds(link.Target, Pattern),
                    page);
            }
        }

        private KeyValuePair<Element, int> FindOptimomAndMinimomStage()
        {
            var links = lastPack == null ? Pattern.Links : Pattern.GetAllAdjucentLinks(lastPack.FixedElements);
            var nodes = lastPack == null ? Pattern.Nodes : Pattern.GetAllAdjucentNodes(lastPack.FixedElements);
            var dict = new Dictionary<Element, int>();
            foreach (var link in links)
            {
                var possibleSources = lastPack == null ? null : lastPack.GetPossibleIds(link.Source, Pattern);
                var possibleTargets = lastPack == null ? null : lastPack.GetPossibleIds(link.Target, Pattern);
                dict[link] = expander.CountLink(link,possibleSources, possibleTargets);
            }
            foreach (var node in nodes)
            {
                var possibleIds = (lastPack == null ? null : lastPack.GetPossibleIds(node.ID, Pattern));
                dict[node] = expander.CountNode(node, possibleIds);
            }
            var min = dict.OrderByDescending(x => x.Value).First();
            return min;
        }

        private void CreateNewPack(Element element, int page, int pageCount)
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
            newPack.Page = page;
            if (pageCount == -1)
            {
                var count = CountElement(element);
                newPack.PageCount = (count - 1) / PageMax + 1;
            }
            else
            {
                newPack.PageCount = pageCount;
            }
        }

        private int CountElement(Element element)
        {
            if (element is Node)
            {
                return expander.CountNode((Node)element, newPack.GetPossibleIds(element.ID, Pattern));
            }
            else
            {
                var link = (Link)element;
                return expander.CountLink(link,
                    newPack.GetPossibleIds(link.Source, Pattern),
                    newPack.GetPossibleIds(link.Target, Pattern));
            }
        }

        private void AddNewPackToStack()
        {
            StackPack.Push(newPack);
            lastPack = newPack;
        }

        private void PopLastPackFromStack()
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
        }



        private void ExpandFirstTimeAndFirstPack()
        {
            var minPair = FindOptimomAndMinimomStage();
            var minElement = minPair.Key;
            var count = minPair.Value;
            ExpandElement(minElement, count);
        }



        private void CombineLastPackWithANewPack()
        {
            var element = lastPack.LastElementExpanded;
            var popedPack = lastPack;
            ChangeLastPagebyNextPage();
            newPack.SetsMap[element.ID] = newPack.SetsMap[element.ID].Union(popedPack.SetsMap[element.ID]).ToList();
            newPack.FixedPatterns = newPack.FixedPatterns.Union(popedPack.FixedPatterns).ToList();
        }


        private void ChangeLastPagebyNextPage()
        {
            var popedPack = StackPack.Peek();
            PopLastPackFromStack();

            var element = popedPack.LastElementExpanded;

            CreateNewPack(element, popedPack.Page + 1, popedPack.PageCount);
            var expandedElements = ExpandLastElementOfNewPack(newPack.Page);
            newPack.SetsMap[element.ID] = expandedElements;
            newPack.FixedElements.Add(element.ID);
            newPack.expandType = popedPack.expandType;
            newPack.UpdateFixedPatterns(element, Pattern);
            newPack.UpdateSetsFromFixedPatterns();

            AddNewPackToStack();
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

/*
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

    */
