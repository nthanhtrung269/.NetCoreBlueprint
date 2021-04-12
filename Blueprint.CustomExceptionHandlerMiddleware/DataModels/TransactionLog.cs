using Blueprint.CustomExceptionHandlerMiddleware.Project.Models;
using System;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project.DataModels
{
    public partial class TransactionLog : Entity<long>
    {
        public Guid? CorrelationId { get; set; }
        public string TransactionName { get; set; }
        public string Status { get; set; }
        public string RequestData { get; set; }
        public string ResponseData { get; set; }
        public string ExceptionMessage { get; set; }
        public string Exception { get; set; }
        public string HostName { get; set; }
        public string AdditionalData { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
