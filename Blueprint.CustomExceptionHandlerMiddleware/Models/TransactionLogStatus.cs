namespace Blueprint.CustomExceptionHandlerMiddleware.Project.Models
{
    public enum TransactionLogStatus
    {
        Ok = 0,
        Accepted = 1,
        Processing = 2,
        Completed = 3,
        Pending = 4,
        Cancelled = 5,
        Failed = 6,
        Error = 7,
        Rejected = 8,
        Unknown = 9
    }
}
