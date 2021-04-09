using System;

namespace Blueprint.Polly
{
    public partial class TransactionLog
    {
        public long TransactionLogId { get; set; }
        public Guid? CorrelationId { get; set; }
        public long TransactionId { get; set; }
        public int StepOrder { get; set; }
        public string StepName { get; set; }
        public string Status { get; set; }
        public long OrderId { get; set; }
        public string UserId { get; set; }
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
