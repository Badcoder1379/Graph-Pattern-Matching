using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatternMatching.Package.elastic
{
    public class SearchParameter
    {
        public string Keyword { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Location { get; set; }
        public long? PriceMin { get; set; }
        public long? PriceMax { get; set; }

        public string Username { get; set; }
    }
}