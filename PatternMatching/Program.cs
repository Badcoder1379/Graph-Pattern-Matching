
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elasticsearch.Net;
using Nest;
using PatternMatching.Package.logic;
using PatternMatching.Package.model;
using Node = PatternMatching.Package.model.Node;

namespace PatternMatching
{
    public class Program
    {
        public static void Main(string[] args)
        {

            string pattern = "../src/3setP/";
            string graph = "../src/3set/";

            //Create3Set(9, 9, 9, graph);
            PMNoramal(graph, pattern);



            //var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("mmdgraph").DisableDirectStreaming();
            //var client = new ElasticClient(settings);
            //PMDB(client, fileName);


            //WriteToDB(client, fileName);

        }


        public static void Create3Set(int a, int b, int c, string fileName)
        {
            StreamWriter sw = new StreamWriter(fileName + "nodes.txt");
            sw.WriteLine(a + b + c);
            List<string> writeNodeSet(char ch, int n)
            {
                var list = new List<string>();
                for (int i = 0; i < a; i++)
                {
                    var id = "dddddddd-dddd-dddd-dddd-dddddddddd" + ch + i.ToString();
                    list.Add(id);
                    sw.WriteLine(id + " N" + Char.ToUpper(ch));
                }
                return list;
            }
            var aSet = writeNodeSet('a', a);
            var bSet = writeNodeSet('b', b);
            var cSet = writeNodeSet('c', c);

            sw.Close();
            sw = new StreamWriter(fileName + "edges.txt");
            sw.WriteLine(a * b + b * c + c * a);
            void writeLinkSet(char ch1, char ch2, List<string> set1, List<string> set2)
            {
                for (int m1 = 0; m1 < set1.Count; m1++)
                {
                    for (int m2 = 0; m2 < set2.Count; m2++)
                    {
                        var id = "eeeeeeee-eeee-eeee-eeee-eeeeeeee" + ch1 + ch2 + m1.ToString() + m2.ToString();
                        var source = set1[m1];
                        var target = set2[m2];
                        var text = id + ' ' + source + ' ' + target + " L";
                        sw.WriteLine(text);
                    }
                }

            }

            writeLinkSet('a','b', aSet, bSet);
            writeLinkSet('b','c', bSet, cSet);
            writeLinkSet('c','a', cSet, aSet);
            sw.Close();
        }


        public static void PMDB(ElasticClient client, string fileName)
        {
            //GraphMaker.Create(1000, 2000, "../src/star4g/");
            //GraphMaker.Create(5, 4, "../src/rp/");

            var sourceManagement = new SourceManagement(client);

            //var graphImporter = new Importer();
            //graphImporter.Import("../src/hard1/");
            //var expander = new Expander(graphImporter);

            var expander = new Expander(sourceManagement);

            var patternImpoter = new Importer();

            var bussiness = new Business(new Pattern(patternImpoter.Import(fileName)), expander);
            bussiness.Run();
            bussiness.PrintResults();
        }

        public static void PMNoramal(string graphFile, string patternFile)
        {
            var graphImporter = new Importer();
            graphImporter.Import(graphFile);
            var expander = new Expander(graphImporter);


            var patternImpoter = new Importer();

            var bussiness = new Business(new Pattern(patternImpoter.Import(patternFile)), expander);
            bussiness.Run();
            bussiness.PrintResults();
        }




        public static void Create(ElasticClient client, int nodesCount, int edgesCount)
        {

            Random rnd = new Random();
            var nodes = new List<Node>();

            var idList = new Guid[nodesCount];
            for (int i = 0; i < nodesCount; i++)
            {
                char randomChar = (char)rnd.Next('A', 'C');
                var node = new Node(Guid.NewGuid(), null, randomChar.ToString());
                idList[i] = node.ID;
                client.IndexDocument(node);
            }

            for (int j = 0; j < edgesCount; j++)
            {
                int r1 = rnd.Next(nodesCount);
                int r2 = rnd.Next(nodesCount);
                if (r1 == r2)
                {
                    j--;
                    continue;
                }
                char randomChar = (char)rnd.Next('A', 'F');
                var resoponse = client.IndexDocument(new Link(Guid.NewGuid(), randomChar.ToString(), idList[r1], idList[r2], null));
            }

        }



        public static void WriteToDB(ElasticClient client, string fileName)
        {
            var graphImporter = new Importer();
            var graph = graphImporter.Import(fileName);

            foreach (var node in graph.Nodes)
            {
                client.IndexDocument(node);
            }
            foreach (var link in graph.Links)
            {
                client.IndexDocument(link);
            }
        }
    }
}
