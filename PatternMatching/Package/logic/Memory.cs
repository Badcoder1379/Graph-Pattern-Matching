using PatternMatching.Package.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.logic
{
    class Memory
    {
        public List<Node> Nodes = new List<Node>();
        public List<Link> Links = new List<Link>();

        public void Read()
        {
            try
            {
                HashSet<Guid> allIDs = new HashSet<Guid>();
                StreamReader sr = new StreamReader("../src/nodes.txt");
                int nodeCount = Int32.Parse(sr.ReadLine());
                for (int i = 0; i < nodeCount; i++)
                {
                    var line = sr.ReadLine().Split(' ');
                    var nodeID = Guid.Parse(line[0]);
                    var label = line[1];
                    Nodes.Add(new Node(nodeID, null, label));
                }
                sr.Close();

                sr = new StreamReader("../src/edges.txt");
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

        }
    public List<Link> GetAllLinks(string label, List<Guid> possibleSources, List<Guid> possibleTargets)
        {
            return Links.Where(link => (link.Label == label) &&
            (possibleSources == null || possibleSources.Contains(link.ID)) &&
            (possibleTargets));
        }
    }
}
