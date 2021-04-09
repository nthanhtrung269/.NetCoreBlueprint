using System;

namespace Blueprint.Polly
{
    public interface ITransactionLogService
    {
        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId);
        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, Guid correlationId);
        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, long orderId, long transactionId, Guid correlationId);
        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string responseData, string userId, long orderId, long transactionId);
        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string responseData, string userId, long orderId, long transactionId, Guid correlationId);
        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, string exceptionMessage, string exception);
        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, string exceptionMessage, string exception, Guid correlationId);
        TransactionLog AddTransactionLog(TransactionLog transactionLog);
    }
}
