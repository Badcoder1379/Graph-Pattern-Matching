



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
        private SourceManagement sourceManagemen;
        private Importer importer = new Importer();
        public bool dataBase = false;

        public Expander(Importer importer)
        {
            this.importer = importer;
        }

        public Expander(SourceManagement sourceManagement)
        {
            this.sourceManagemen = sourceManagemen;
        }

        public int CountLink(Link link, List<Guid> possibleSources, List<Guid> possibleTargets)
        {
            if (dataBase)
            {
                return 0;
            }
            else
            {
                return importer.GetAllLinks(link.Label, possibleSources, possibleTargets).Count;
            }
        }

        public int CountNode(Node node, List<Guid> possibleIds)
        {
            if (dataBase)
            {
                return 0;
            }
            else
            {
                return importer.GetAllNodes(node.Label, possibleIds).Count;
            }
        }


        public List<Element> ExpandNode(Node node, List<Guid> possibleIds, int pageNumber)
        {
            if (dataBase)
            {
                return null;
            }
            else
            {
                return importer.ImportNodes(node.Label, possibleIds, pageNumber);
            }

        }

        public List<Element> ExpandLink(Link link, List<Guid> possibleSpurces, List<Guid> possibleTargets, int pageNumber)
        {
            if (dataBase)
            {
                return null;
            }
            else
            {
                return importer.ImportLinks(link.Label, possibleSpurces, possibleTargets, pageNumber);
            }

        }
    }
}
