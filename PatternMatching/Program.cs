
using System;
using System.Collections.Generic;
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

            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("mmdgraph").DisableDirectStreaming();
            var client = new ElasticClient(settings);

            //var graphImporter = new Importer();
            //var graph = graphImporter.Import("../src/p2/");
            //WriteToDB(client, graph);

            //Create(client, 100, 100);

            PM(client);
            return;

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

        public static void PM(ElasticClient client)
        {
            //GraphMaker.Create(1000, 2000, "../src/star4g/");
            //GraphMaker.Create(5, 4, "../src/rp/");

            var sourceManagement = new SourceManagement(client);

            //var graphImporter = new Importer();
            //graphImporter.Import("../src/hard1/");
            //var expander = new Expander(graphImporter);

            var expander = new Expander(sourceManagement);

            var patternImpoter = new Importer();

            var bussiness = new Business(new Pattern(patternImpoter.Import("../src/p2/")), expander);
            bussiness.Run();
            bussiness.PrintResults();
        }


        public static void WriteToDB(ElasticClient client,Graph graph)
        {
            foreach(var node in graph.Nodes)
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
