
using System;
using PatternMatching.Package.logic;
using PatternMatching.Package.model;

namespace PatternMatching
{
    class Program
    {
        static void Main(string[] args)
        {
            //GraphMaker.Create(1000, 2000, "../src/graph/");
            //GraphMaker.Create(4, 3, "../src/pattern/");
            var graphImporter = new Importer();
            graphImporter.Import("../src/graphTri/");
            var expander = new Expander(graphImporter);

            var patternImpoter = new Importer();

            var bussiness = new Business(new Pattern(patternImpoter.Import("../src/graphTri/")), expander);
            bussiness.Run();
            bussiness.PrintResults();
        }
    }
}
