



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
        private SourceManagement sourceManagement;
        private Importer importer = new Importer();
        public bool dataBase = false;

        public Expander(Importer importer)
        {
            this.importer = importer;
        }

        public Expander(SourceManagement sourceManagement)
        {
            this.sourceManagement  = sourceManagement;
            this.dataBase = true;
        }

        public int CountLink(Link link, List<Guid> possibleSources, List<Guid> possibleTargets)
        {
            if (dataBase)
            {
                return sourceManagement.CountLink(link, possibleSources, possibleTargets);
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
                return sourceManagement.CountNode(node, possibleIds);
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
                return sourceManagement.GetNodes(node, possibleIds, pageNumber).Cast<Element>().ToList();
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
                return sourceManagement.GetLinks(link, possibleSpurces, possibleTargets, pageNumber).Cast<Element>().ToList();
            }
            else
            {
                return importer.ImportLinks(link.Label, possibleSpurces, possibleTargets, pageNumber);
            }

        }
    }
}
