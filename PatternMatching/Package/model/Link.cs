using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching.Package.model
{
    public class Link : Element
    {
        public Guid Source;
        public Guid Target;

        public Link(Guid ID, string label, Guid Source, Guid Target, Property property)
        {
            this.ID = ID;
            this.Source = Source;
            this.Target = Target;
            Property = property;
            Label = label;
        }
    }
}
