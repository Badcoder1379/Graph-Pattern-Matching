
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



            var maxList = new[] { 1, 2, 3, 4, 5 };
            var counterType = "counter-type";
            var lowerBound = DateTime.UtcNow.Date.AddDays(-7);

            Term(client, "d69bea0f-ceab-46fb-b9b4-fc8527df3d3f", "D");
            return;


            /*
            var queryResult = client.Search<String>(s => s
                .Query(q => +q.Term(x => x.Type, counterType) && +q
                    .Terms(t => t
                        .Field(x => x.CounterId)
                        .Terms(maxList)
                    )
                )
                .AllTypes()
                .Scroll("1m")
                .Size(10000)
            );
            */

            //Create(client, 3, 3);
            //GetAll(client);
            GetNodeByID(client, "e8cfa0af-867b-46e4-8a7f-77a950daa737", "A");
        }

        internal static void GetNodeByList(ElasticClient client, string id, string label)
        {


            var response = client.Search<Node>(s => s.Query(q => +q.Match(m => m
                .Field(f => f.ID)
                .Query(id)
                ) && +q.Match(m => m.Field(f => f.Label).Query(label))
                    && +q.Terms(c => c
                    .Name("named_query")
                    .Boost(1.1)
                    .Field(p => p.Label)
                    .Terms(label)
                )));

        }


        internal static void Term(ElasticClient client, string id, string label)
        {
            var possibleIDs = new List<string>();
            possibleIDs.Add(id);

            var labelQuery = new MatchQuery
            {
                Field = "label",
                Query = label
            };

            var idsQuery = new TermsQuery
            {
                Field = "iD.keyword",
                Terms = possibleIDs
            };

            var query = new BoolQuery()
            {
                Filter = new List<QueryContainer>
                {
                    labelQuery,
                    idsQuery
                }
            };

            var result = client.Search<Node>(x => x
                .Query(q => query)
            /*
            .Index()
            .From()
            .Size()
            */
            );
        }

        internal static void GetNodeByID(ElasticClient client, string id, string label)
        {
            var response = client.Search<Node>(s => s.Query(q => +q.Match(m => m
                .Field(f => f.ID)
                .Query(id)
                ) && +q.Match(m => m.Field(f => f.Label).Query(label))));

            var node = response.Hits.First().Source;

            var list = new List<string>();
            list.Add(id);

            var countReponse = client.Count<Node>(c => c.Query(q => +q.Terms(t => t.Field(x => x.ID).Terms(list))));

            var count = countReponse.Count;

        }



        public static void GetAll(ElasticClient client)
        {
            var response = client.Search<Node>();


            var results = response.Documents.ToList();
            foreach (var result in results)
            {
                Console.WriteLine(result.ID);
            }
        }

        public static void Create(ElasticClient client, int nodesCount, int edgesCount)
        {

            Random rnd = new Random();
            var nodes = new List<Node>();

            var idList = new Guid[nodesCount];
            for (int i = 0; i < nodesCount; i++)
            {
                char randomChar = (char)rnd.Next('A', 'F');
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

            var bussiness = new Business(new Pattern(patternImpoter.Import("../src/p0/")), expander);
            bussiness.Run();
            bussiness.PrintResults();
        }
    }
}
