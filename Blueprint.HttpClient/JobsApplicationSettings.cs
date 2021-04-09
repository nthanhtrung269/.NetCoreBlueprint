namespace Blueprint.HttpClient1
{
    public class JobsApplicationSettings
    {
        public string Environment { get; set; }

        public RequestApi AuthorizationProvider1Api { get; set; }

        public RequestApi ProductProvider1Api { get; set; }

        public RequestApi ProductProvider2Api { get; set; }
    }

    public class RequestApi
    {
        public string ProviderName { get; set; }

        public string RequestUri { get; set; }

        public string Authorization { get; set; }
    }
}
