using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatternMatching.Package.elastic
{
    internal class EsClient
    {
        private static Dictionary<string, ElasticClient> innerClients = new Dictionary<string, ElasticClient>();
        public static ElasticClient GetInstance(string indexName)
        {
            if (!innerClients.ContainsKey(indexName))
            {
                //var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("bekhar").DisableDirectStreaming();

                var settings = new ConnectionSettings(new Uri("https://elastic:1IeomFYO4yVyqzlrO95SpXFy@a8769f2176114de3ac5fb6371bd61b21.eu-central-1.aws.cloud.es.io:9243")).DefaultIndex(indexName).DisableDirectStreaming();
                innerClients.Add(indexName, new ElasticClient(settings));
            }

            return innerClients[indexName];
        }

    }
}