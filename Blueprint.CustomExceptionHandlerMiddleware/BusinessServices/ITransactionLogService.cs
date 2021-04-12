using Blueprint.CustomExceptionHandlerMiddleware.Project.DataModels;
using Blueprint.CustomExceptionHandlerMiddleware.Project.Models;
using System;
using System.Threading.Tasks;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project.BusinessServices
{
    public interface ITransactionLogService
    {
        Task<TransactionLog> AddTransactionLogAsync(TransactionLog transactionLog);

        Task<TransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string exceptionMessage, string exception, Guid correlationId);

        Task AddTransactionLogExceptionAsync(TransactionLogStep transactionLogStep, BaseRequestObject request, Exception exception);
    }
}
