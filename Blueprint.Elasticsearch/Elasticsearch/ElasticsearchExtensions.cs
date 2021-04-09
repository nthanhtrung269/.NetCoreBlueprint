using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Blueprint.Elasticsearch
{
    public static class ElasticsearchExtensions
    {
        /// <summary>
        /// Adds ElasticLowLevelClient to services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="applicationSettings">The applicationSettings.</param>
        public static void AddElasticsearch(this IServiceCollection services, ApplicationSettings applicationSettings)
        {
            var uris = applicationSettings.Elasticsearch.Hosts.Select(s => new Uri(s)).ToArray();
            var connectionPool = new SniffingConnectionPool(uris);
            var settings = new ConnectionConfiguration(connectionPool);
            var client = new ElasticLowLevelClient(settings);

            services.AddSingleton<IElasticLowLevelClient>(client);
        }
    }
}
