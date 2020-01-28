



using PatternMatching.Package.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.logic
{
    public class Expander
    {
        public SourceManagement sourceManagemen;
        public Importer Importer = new Importer();

        public Expander(Importer importer)
        {
            Importer = importer;
        }

        public int CountLink(Link link, List<Guid> possibleSources, List<Guid> possibleTargets)
        {
            return Importer.GetAllLinks(link.Label, possibleSources, possibleTargets).Count;
        }
        public int CountNode(Node node, List<Guid> possibleIds)
        {
            return Importer.GetAllNodes(node.Label, possibleIds).Count;
        }

        public List<Element> ExpandNode(Node node, List<Guid> possibleIds, int pageNumber)
        {
            return Importer.ImportNodes(node.Label, possibleIds, pageNumber);
        }

        public List<Element> ExpandLink(Link link, List<Guid> possibleSpurces, List<Guid> possibleTargets, int pageNumber)
        {
            return Importer.ImportLinks(link.Label, possibleSpurces, possibleTargets, pageNumber);
        }
    }
}
