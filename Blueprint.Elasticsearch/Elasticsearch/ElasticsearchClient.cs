using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Blueprint.Elasticsearch
{
    public class ElasticsearchClient : IElasticsearchClient
    {
        private readonly ILogger<ElasticsearchClient> _logger;
        private readonly IElasticLowLevelClient _elasticLowLevelClient;

        public ElasticsearchClient(ILogger<ElasticsearchClient> logger, IElasticLowLevelClient elasticLowLevelClient)
        {
            _logger = logger;
            _elasticLowLevelClient = elasticLowLevelClient;
        }

        /// <summary>
        /// Adds Index.
        /// </summary>
        /// <param name="dataToIndex">The dataToIndex.</param>
        /// <param name="indexName">The indexName.</param>
        /// <param name="id">The id.</param>
        /// <returns>System.Boolean.</returns>
        public bool AddIndex(object dataToIndex, string indexName, string id)
        {
            try
            {
                _logger.LogInformation($"Start AddIndex to Elasticsearch with index {indexName}.");
                var timer = Stopwatch.StartNew();
                var response = _elasticLowLevelClient.Index<StringResponse>(indexName, id, PostData.Serializable(dataToIndex));
                timer.Stop();

                _logger.LogInformation($"End AddIndex to Elasticsearch with index {indexName} after {timer.ElapsedMilliseconds}ms.");
                return response.Success;
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"AddIndex to {indexName} failed.", ex);
                throw;
            }
        }
    }
}
