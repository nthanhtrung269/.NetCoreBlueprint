using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Enums;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using System;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces
{
    public interface ITransactionLogService
    {
        Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId);
        Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, Guid correlationId);
        Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, long orderId, long transactionId, Guid correlationId);
        Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string responseData, string userId, long orderId, long transactionId);
        Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string responseData, string userId, long orderId, long transactionId, Guid correlationId);
        Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, string exceptionMessage, string exception);
        Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep, TransactionLogStatus transactionLogStatus, string requestData, string userId, string exceptionMessage, string exception, Guid correlationId);
        Task<BlueprintTransactionLog> AddTransactionLogAsync(BlueprintTransactionLog transactionLog);
    }
}