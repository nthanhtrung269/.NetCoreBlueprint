using Couchbase;
using Couchbase.IO;
using System;
using System.Runtime.Serialization;

namespace Blueprint.Couchbase
{
    public class CouchbaseException : Exception
    {
        public CouchbaseException()
        {
        }

        public CouchbaseException(IOperationResult result)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
        }

        public CouchbaseException(IDocumentResult result)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
        }

        public CouchbaseException(IOperationResult result, string key)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
            Key = key;
        }

        public CouchbaseException(IDocumentResult result, string key)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
            Key = key;
        }

        public CouchbaseException(string message)
            : base(message)
        {
        }

        public CouchbaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CouchbaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ResponseStatus Status { get; set; }

        public string Key { get; set; }
    }
}
