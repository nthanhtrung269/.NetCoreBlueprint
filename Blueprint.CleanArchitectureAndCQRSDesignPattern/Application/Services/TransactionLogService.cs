using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Constants;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Enums;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Services
{
    public class TransactionLogService : ITransactionLogService
    {
        private readonly ILogger<TransactionLogService> _logger;
        private readonly ITransactionLogRepository _transactionLogRepository;

        public TransactionLogService(ILogger<TransactionLogService> logger,
            ITransactionLogRepository transactionLogRepository)
        {
            _logger = logger;
            _transactionLogRepository = transactionLogRepository;
        }

        /// <summary>
        /// Adds transaction log.
        /// </summary>
        /// <param name="transactionLogStep">The transactionLogStep.</param>
        /// <param name="transactionLogStatus">The transactionLogStatus.</param>
        /// <param name="requestData">The requestData.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>RsTransactionLog.</returns>
        public async Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep,
            TransactionLogStatus transactionLogStatus,
            string requestData,
            string userId
            )
        {
            BlueprintTransactionLog transactionLog = new BlueprintTransactionLog()
            {
                StepOrder = (int)transactionLogStep,
                StepName = transactionLogStep.ToString(),
                Status = transactionLogStatus.ToString(),
                UserId = userId,
                RequestData = requestData
            };

            return await AddTransactionLogAsync(transactionLog);
        }

        /// <summary>
        /// Adds transaction log.
        /// </summary>
        /// <param name="transactionLogStep">The transactionLogStep.</param>
        /// <param name="transactionLogStatus">The transactionLogStatus.</param>
        /// <param name="requestData">The requestData.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="correlationId">The correlationId.</param>
        /// <returns>RsTransactionLog.</returns>
        public async Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep,
            TransactionLogStatus transactionLogStatus,
            string requestData,
            string userId,
            Guid correlationId
            )
        {
            BlueprintTransactionLog transactionLog = new BlueprintTransactionLog()
            {
                StepOrder = (int)transactionLogStep,
                StepName = transactionLogStep.ToString(),
                Status = transactionLogStatus.ToString(),
                UserId = userId,
                RequestData = requestData,
                CorrelationId = correlationId
            };

            return await AddTransactionLogAsync(transactionLog);
        }

        /// <summary>
        /// Adds transaction log.
        /// </summary>
        /// <param name="transactionLogStep">The transactionLogStep.</param>
        /// <param name="transactionLogStatus">The transactionLogStatus.</param>
        /// <param name="requestData">The requestData.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="orderId">The orderId.</param>
        /// <param name="transactionId">The transactionId.</param>
        /// <param name="correlationId">The correlationId.</param>
        /// <returns>RsTransactionLog.</returns>
        public async Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep,
            TransactionLogStatus transactionLogStatus,
            string requestData,
            string userId,
            long orderId,
            long transactionId,
            Guid correlationId
            )
        {
            BlueprintTransactionLog transactionLog = new BlueprintTransactionLog()
            {
                StepOrder = (int)transactionLogStep,
                StepName = transactionLogStep.ToString(),
                Status = transactionLogStatus.ToString(),
                UserId = userId,
                OrderId = orderId,
                TransactionId = transactionId,
                RequestData = requestData,
                CorrelationId = correlationId
            };

            return await AddTransactionLogAsync(transactionLog);
        }

        /// <summary>
        /// Adds transaction log.
        /// </summary>
        /// <param name="transactionLogStep">The transactionLogStep.</param>
        /// <param name="transactionLogStatus">The transactionLogStatus.</param>
        /// <param name="requestData">The requestData.</param>
        /// <param name="responseData">The responseData.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="orderId">The orderId.</param>
        /// <param name="transactionId">The transactionId.</param>
        /// <returns>RsTransactionLog.</returns>
        public async Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep,
            TransactionLogStatus transactionLogStatus,
            string requestData,
            string responseData,
            string userId,
            long orderId,
            long transactionId
            )
        {
            BlueprintTransactionLog transactionLog = new BlueprintTransactionLog()
            {
                StepOrder = (int)transactionLogStep,
                StepName = transactionLogStep.ToString(),
                Status = transactionLogStatus.ToString(),
                UserId = userId,
                OrderId = orderId,
                TransactionId = transactionId,
                RequestData = requestData,
                ResponseData = responseData
            };

            return await AddTransactionLogAsync(transactionLog);
        }

        /// <summary>
        /// Adds transaction log.
        /// </summary>
        /// <param name="transactionLogStep">The transactionLogStep.</param>
        /// <param name="transactionLogStatus">The transactionLogStatus.</param>
        /// <param name="requestData">The requestData.</param>
        /// <param name="responseData">The responseData.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="orderId">The orderId.</param>
        /// <param name="transactionId">The transactionId.</param>
        /// <param name="correlationId">The correlationId.</param>
        /// <returns>RsTransactionLog.</returns>
        public async Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep,
            TransactionLogStatus transactionLogStatus,
            string requestData,
            string responseData,
            string userId,
            long orderId,
            long transactionId,
            Guid correlationId
            )
        {
            BlueprintTransactionLog transactionLog = new BlueprintTransactionLog()
            {
                StepOrder = (int)transactionLogStep,
                StepName = transactionLogStep.ToString(),
                Status = transactionLogStatus.ToString(),
                UserId = userId,
                OrderId = orderId,
                TransactionId = transactionId,
                RequestData = requestData,
                ResponseData = responseData,
                CorrelationId = correlationId
            };

            return await AddTransactionLogAsync(transactionLog);
        }

        /// <summary>
        /// Adds transaction log.
        /// </summary>
        /// <param name="transactionLogStep">The transactionLogStep.</param>
        /// <param name="transactionLogStatus">The transactionLogStatus.</param>
        /// <param name="requestData">The requestData.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="exceptionMessage">The exceptionMessage.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>RsTransactionLog.</returns>
        public async Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep,
            TransactionLogStatus transactionLogStatus,
            string requestData,
            string userId,
            string exceptionMessage,
            string exception
            )
        {
            BlueprintTransactionLog transactionLog = new BlueprintTransactionLog()
            {
                StepOrder = (int)transactionLogStep,
                StepName = transactionLogStep.ToString(),
                Status = transactionLogStatus.ToString(),
                UserId = userId,
                RequestData = requestData,
                ExceptionMessage = exceptionMessage,
                Exception = exception
            };

            return await AddTransactionLogAsync(transactionLog);
        }

        /// <summary>
        /// Adds transaction log.
        /// </summary>
        /// <param name="transactionLogStep">The transactionLogStep.</param>
        /// <param name="transactionLogStatus">The transactionLogStatus.</param>
        /// <param name="requestData">The requestData.</param>
        /// <param name="userId">The userId.</param>
        /// <param name="exceptionMessage">The exceptionMessage.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="correlationId">The correlationId.</param>
        /// <returns>RsTransactionLog.</returns>
        public async Task<BlueprintTransactionLog> AddTransactionLogAsync(TransactionLogStep transactionLogStep,
            TransactionLogStatus transactionLogStatus,
            string requestData,
            string userId,
            string exceptionMessage,
            string exception,
            Guid correlationId
            )
        {
            BlueprintTransactionLog transactionLog = new BlueprintTransactionLog()
            {
                CorrelationId = correlationId,
                StepOrder = (int)transactionLogStep,
                StepName = transactionLogStep.ToString(),
                Status = transactionLogStatus.ToString(),
                UserId = userId,
                RequestData = requestData,
                ExceptionMessage = exceptionMessage,
                Exception = exception
            };

            return await AddTransactionLogAsync(transactionLog);
        }

        /// <summary>
        /// Adds transaction log.
        /// </summary>
        /// <param name="transactionLog">The transactionLog.</param>
        /// <returns>RsTransactionLog.</returns>
        public async Task<BlueprintTransactionLog> AddTransactionLogAsync(BlueprintTransactionLog transactionLog)
        {
            try
            {
                if (transactionLog.CorrelationId == Guid.Empty || transactionLog.CorrelationId == null)
                {
                    transactionLog.CorrelationId = Guid.NewGuid();
                }

                transactionLog.HostName = Dns.GetHostName();
                transactionLog.CreatedBy = AuthorizationConstants.SITE_ADMIN_ROLE;
                transactionLog.CreatedDate = DateTime.UtcNow;
                transactionLog.ModifiedBy = AuthorizationConstants.SITE_ADMIN_ROLE;
                transactionLog.ModifiedDate = DateTime.UtcNow;

                _logger.LogInformation($"Call to {nameof(AddTransactionLogAsync)} with LogData: {JsonConvert.SerializeObject(transactionLog)}.");

                await _transactionLogRepository.AddAsync(transactionLog);
                await _transactionLogRepository.CommitAsync();

                return transactionLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Call to {nameof(AddTransactionLogAsync)} failed.");
                return transactionLog;
            }
        }
    }
}