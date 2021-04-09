using System;

namespace Blueprint.Polly
{
    public class TransactionLogService : ITransactionLogService
    {
        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId)
        {
            throw new NotImplementedException();
        }

        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, long orderId, long transactionId, Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string responseData, string userId, long orderId, long transactionId)
        {
            throw new NotImplementedException();
        }

        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string responseData, string userId, long orderId, long transactionId, Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, string exceptionMessage, string exception)
        {
            throw new NotImplementedException();
        }

        public TransactionLog AddTransactionLog(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, string exceptionMessage, string exception, Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public TransactionLog AddTransactionLog(TransactionLog transactionLog)
        {
            throw new NotImplementedException();
        }
    }
}
