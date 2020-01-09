
using System;
using PatternMatching.Package.logic;
using PatternMatching.Package.model;

namespace PatternMatching
{
    class Program
    {
        static void Main(string[] args)
        {
            GraphMaker.Create(1000, 1000, "../src/graph/");
            // GraphMaker.Create(1, 0, "../src/pattern/");
            var graphImporter = new Importer();
            graphImporter.Import("../src/graph/");
            var expander = new Expander(graphImporter);

            var patternImpoter = new Importer();

            var bussiness = new Business(new Pattern(patternImpoter.Import("../src/pattern2/")), expander);
            bussiness.Run();
            bussiness.PrintResults();
            
            
        }
    }
}
