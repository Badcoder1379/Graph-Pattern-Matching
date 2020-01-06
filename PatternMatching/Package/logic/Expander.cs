



using PatternMatching.Package.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.logic
{
    class Expander
    {
        public Pattern Pattern;
        public SourceManagement sourceManagemen;

        public int CountLink(Link link, List<Guid> possibleSources, List<Guid> possibleTargets)
        {
            return 0;
        }
        public int CountNode(Node node, List<Guid> possibleIds)
        {
            return 0;
        }

        public HashSet<Element> ExpandNode(Node node, List<Guid> possibleIds, int pageNumber)
        {
            return null;
        }

        public List<Element> ExpandLink(Link link, List<Guid> possibleSpurces, List<Guid> possibleTargets, int pageNumber)
        {
            return null;
        }
    }
}
