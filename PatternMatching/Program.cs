
using System;
using PatternMatching.Package.logic;
using PatternMatching.Package.model;

namespace PatternMatching
{
    class Program
    {
        static void Main(string[] args)
        {
            //GraphMaker.Create(1000, 2000, "../src/star4g/");
            //GraphMaker.Create(5, 4, "../src/rp/");
            var graphImporter = new Importer();
            graphImporter.Import("../src/tridom/");
            var expander = new Expander(graphImporter);

            var patternImpoter = new Importer();

            var bussiness = new Business(new Pattern(patternImpoter.Import("../src/tri/")), expander);
            bussiness.Run();
            bussiness.PrintResults();
        }
    }
}
