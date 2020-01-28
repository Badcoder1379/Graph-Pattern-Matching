using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PatternMatching.Package.logic;
using PatternMatching.Package.model;


namespace PatternMatching.Tests
{
    [TestClass()]
    public class ProgramTests
    {

        [TestMethod]
        public void p1()
        {
            testWithFileName("p1");
        }
        [TestMethod]
        public void p2()
        {
            testWithFileName("p2");

        }
        [TestMethod]
        public void p3()
        {
            testWithFileName("p3");

        }
        [TestMethod]
        public void tri()
        {
            testWithFileName("tri");

        }


        public void testWithFileName(string fileName)
        {
            var graphImporter = new Importer();
            graphImporter.Import("../src/" + fileName + "/");
            var expander = new Expander(graphImporter);

            var patternImpoter = new Importer();

            var bussiness = new Business(new Pattern(patternImpoter.Import("../src/" + fileName + "/")), expander);
            bussiness.Run();
            Assert.IsTrue(bussiness.Results.Count > 0);
        }
    }
}