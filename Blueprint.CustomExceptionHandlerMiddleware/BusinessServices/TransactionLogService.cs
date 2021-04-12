using Blueprint.CustomExceptionHandlerMiddleware.Project.DataModels;
using Blueprint.CustomExceptionHandlerMiddleware.Project.Models;
using System;
using System.Threading.Tasks;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project.BusinessServices
{
    public class TransactionLogService : ITransactionLogService
    {
        public Task<TransactionLog> AddTransactionLogAsync(TransactionLog transactionLog)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string exceptionMessage, string exception, Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public Task AddTransactionLogExceptionAsync(TransactionLogStep transactionLogStep, BaseRequestObject request, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
