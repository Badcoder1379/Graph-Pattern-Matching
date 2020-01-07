using PatternMatching.Package.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.logic
{
    class Importer
    {
        public List<Node> Nodes = new List<Node>();
        public List<Link> Links = new List<Link>();
        public Graph Import(string address)
        {
            Nodes = new List<Node>();
            Links = new List<Link>();
            try
            {
                HashSet<Guid> allIDs = new HashSet<Guid>();
                StreamReader sr = new StreamReader(address + "nodes.txt");
                int nodeCount = Int32.Parse(sr.ReadLine());
                for (int i = 0; i < nodeCount; i++)
                {
                    var line = sr.ReadLine().Split(' ');
                    var nodeID = Guid.Parse(line[0]);
                    var label = line[1];
                    Nodes.Add(new Node(nodeID, null, label));
                }
                sr.Close();

                sr = new StreamReader(address + "edges.txt");
                var edgeCount = Int32.Parse(sr.ReadLine());
                for (int i = 0; i < edgeCount; i++)
                {
                    var line = sr.ReadLine().Split(' ');
                    var linkID = Guid.Parse(line[0]);
                    var source = Guid.Parse(line[1]);
                    var target = Guid.Parse(line[2]);
                    var label = line[3];
                    Links.Add(new Link(linkID, label, source, target, null));
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("error");
            }
            return new Graph(Nodes, Links);

        }
        public List<Element> GetAllLinks(string label, List<Guid> possibleSources, List<Guid> possibleTargets)
        {
            return Links.Where(link => (link.Label == label) &&
            (possibleSources == null || possibleSources.Contains(link.ID)) &&
            (possibleTargets == null || possibleTargets.Contains(link.Target))).Select(link => (Element)link).ToList();
        }
        public List<Element> GetAllNodes(string label, List<Guid> possibleIDs)
        {
            return Nodes.Where(node => node.Label == label && (possibleIDs == null || possibleIDs.Contains(node.ID))).Select(node => (Element)node).ToList();
        }


        public List<Element> ImportNodes(string label, List<Guid> possibleIDs, int pageNumber)
        {
            var list = GetAllNodes(label, possibleIDs);
            var pageMax = Business.PageMax;
            var first = (pageNumber - 1) * pageMax;
            var elementsCount = Math.Min(list.Count - (pageNumber - 1) * pageMax, pageMax);
            return list.GetRange(first, elementsCount);
        }

        public List<Element> ImportLinks(string label, List<Guid> possibleSources, List<Guid> possibleTargets, int pageNumber)
        {
            var list = GetAllLinks(label, possibleSources, possibleTargets);
            var pageMax = Business.PageMax;
            var first = (pageNumber - 1) * pageMax;
            var elementsCount = Math.Min(list.Count - (pageNumber - 1) * pageMax, pageMax);
            return list.GetRange(first, elementsCount);
        }


    }

}

