namespace Blueprint.Elasticsearch
{
    public interface IElasticsearchClient
    {
        bool AddIndex(object dataToIndex, string indexName, string id);
    }
}
