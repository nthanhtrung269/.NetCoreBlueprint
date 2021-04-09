namespace Blueprint.Elasticsearch
{
    public class ApplicationSettings
    {
        public ElasticsearchSettings Elasticsearch { get; set; }
    }

    public class ElasticsearchSettings
    {
        public string[] Hosts { get; set; }

        public string IndexName { get; set; }
    }
}
