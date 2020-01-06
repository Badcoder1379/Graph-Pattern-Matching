using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.model
{
    class Node : Element
    {

        public Node(Guid iD, Property property, string label)
        {
            ID = iD;
            Property = property;
            Label = label;
        }
    }
}
