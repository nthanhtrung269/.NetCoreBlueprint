using System.Collections.Generic;

namespace Blueprint.Couchbase
{
    public class CouchbaseConfig
    {
        public List<string> Uris { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Bucket { get; set; }
    }
}
