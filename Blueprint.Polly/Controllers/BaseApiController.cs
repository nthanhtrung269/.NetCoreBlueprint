using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Blueprint.Polly.Controllers
{
    /// <summary>
    /// Base class for ApiController derived classes.
    /// </summary>
    public class BaseApiController : ControllerBase
    {
        private readonly ILogger<BaseApiController> _logger;
        private readonly ITransactionLogService _transactionLogService;
        private readonly ICachingService _cachingService;
        private string ControllerName { get; }
        private string InvalidRequestLogMessage => $"{ControllerName} request is invalid";
        private string RequestCompletedLogMessage => $"{ControllerName} successful";

        /// <summary>
        /// Constructor for BaseApiController.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="transactionLogService">The transactionLogService.</param>
        /// <param name="cachingService">The cachingService.</param>
        public BaseApiController(ILogger<BaseApiController> logger,
            ITransactionLogService transactionLogService,
            ICachingService cachingService)
        {
            _logger = logger;
            _transactionLogService = transactionLogService;
            _cachingService = cachingService;

            var controller = GetType().Name;
            ControllerName = controller.Substring(0, controller.Length - "Controller".Length);
        }

        /// <summary>
        /// Validates and processes request.
        /// </summary>
        /// <typeparam name="TRequest">The TRequest.</typeparam>
        /// <typeparam name="TResponse">The TResponse.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="executeRequest">The executeRequest.</param>
        /// <returns>TResponse.</returns>
        protected TResponse ValidateAndProcessRequest<TRequest, TResponse>(TRequest request, Func<TRequest, TResponse> executeRequest)
            where TResponse : BaseResponseObject, new()
        {
            _logger.LogInformation($"Calling {nameof(ValidateAndProcessRequest)} with request: {JsonConvert.SerializeObject(request)}");
            TResponse response = ValidateRequest<TRequest, TResponse>(request);

            if (response != null)
            {
                return response;
            }

            return ProcessRequest(request, executeRequest);
        }

        /// <summary>
        /// Validates and processes request.
        /// </summary>
        /// <typeparam name="TRequest">The TRequest.</typeparam>
        /// <typeparam name="TResponse">The TResponse.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="executeRequest">The executeRequest.</param>
        /// <returns>TResponse.</returns>
        protected TResponse ValidateAndProcessRequestWithGuard<TRequest, TResponse>(TRequest request, Func<TRequest, TResponse> executeRequest)
        {
            _logger.LogInformation($"Calling {nameof(ValidateAndProcessRequestWithGuard)} with request: {JsonConvert.SerializeObject(request)}");
            ValidateRequestWithGuard(request);
            return ProcessRequest(request, executeRequest);
        }

        /// <summary>
        /// Validates and processes request async.
        /// </summary>
        /// <typeparam name="TRequest">The TRequest.</typeparam>
        /// <typeparam name="TResponse">The TResponse.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="executeRequest">The executeRequest.</param>
        /// <returns>Task{TResponse}.</returns>
        protected async Task<TResponse> ValidateAndProcessRequestWithGuardAsync<TRequest, TResponse>(TRequest request, Func<TRequest, Task<TResponse>> executeRequest)
        {
            _logger.LogInformation($"Calling {nameof(ValidateAndProcessRequestWithGuard)} with request: {JsonConvert.SerializeObject(request)}");
            ValidateRequestWithGuard(request);
            return await ProcessRequestAsync(request, executeRequest);
        }

        private TResponse ValidateRequest<TRequest, TResponse>(TRequest request)
            where TResponse : BaseResponseObject, new()
        {
            if (request == null)
            {
                _logger.LogWarning($"{InvalidRequestLogMessage} with Request.Body: {JsonConvert.SerializeObject(new { Request = Request.Body })}");

                return new TResponse()
                {
                    Message = MessageConstants.BadRequest,
                    ErrorCode = ResponseErrorCode.ArgumentNullException,
                    Status = false
                };
            }

            return null;
        }

        private void ValidateRequestWithGuard<TRequest>(TRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning($"{InvalidRequestLogMessage} with Request.Body: {JsonConvert.SerializeObject(new { Request = Request.Body })}");
            }

            Guard.AgainstNull(nameof(TRequest), request);
        }

        private Task<TResponse> ProcessRequestAsync<TRequest, TResponse>(TRequest request, Func<TRequest, Task<TResponse>> executeRequest)
        {
            return ProcessRequestInternalAsync(request, async req =>
            {
                TResponse response = await executeRequest(req);

                return new ActionResponse<TResponse>
                {
                    LogMessage = RequestCompletedLogMessage,
                    ResponseObject = response
                };
            });
        }

        private TResponse ProcessRequest<TRequest, TResponse>(TRequest request, Func<TRequest, TResponse> executeRequest)
        {
            return ProcessRequest(request, req =>
            {
                TResponse response = executeRequest(req);

                return new ActionResponse<TResponse>
                {
                    LogMessage = RequestCompletedLogMessage,
                    ResponseObject = response
                };
            });
        }

        private TResponse ProcessRequest<TRequest, TResponse>(TRequest request, Func<TRequest, ActionResponse<TResponse>> executeRequest)
        {
            try
            {
                ActionResponse<TResponse> executionResult = executeRequest(request);
                var logObject = new
                {
                    Request = request,
                    Response = executionResult.ResponseObject
                };
                _logger.LogInformation($"{executionResult.LogMessage} with Request/Response: {JsonConvert.SerializeObject(logObject)}");

                return executionResult.ResponseObject;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ControllerName} encountered an unexpected error: {JsonConvert.SerializeObject(ex)}");

                if (_cachingService.IsLoggingDatabase())
                {
                    _transactionLogService.AddTransactionLog(TransactionLogStep.BaseApiControllerExceptionHandling,
                    TransactionLogStatus.Completed,
                    JsonConvert.SerializeObject(request),
                    string.Empty,
                    ex.Message,
                    JsonConvert.SerializeObject(ex));
                }

                throw;
            }
        }

        private async Task<TResponse> ProcessRequestInternalAsync<TRequest, TResponse>(TRequest request, Func<TRequest, Task<ActionResponse<TResponse>>> executeRequest)
        {
            _logger.LogDebug($"{ControllerName} controller", request);

            Exception exception = null;
            TResponse response = default;
            var message = "{@ProcessRequest} ";
            var statusCode = 200;
            var timer = Stopwatch.StartNew();
            try
            {
                ActionResponse<TResponse> executionResult = await executeRequest(request);
                message += executionResult.LogMessage;
                response = executionResult.ResponseObject;

                return executionResult.ResponseObject;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ControllerName} encountered an unexpected error: {JsonConvert.SerializeObject(ex)}");

                if (_cachingService.IsLoggingDatabase())
                {
                    _transactionLogService.AddTransactionLog(TransactionLogStep.BaseApiControllerExceptionHandling,
                    TransactionLogStatus.Completed,
                    JsonConvert.SerializeObject(request),
                    string.Empty,
                    ex.Message,
                    JsonConvert.SerializeObject(ex));
                }

                throw;
            }
            finally
            {
                timer.Stop();
                var log = new
                {
                    ControllerName = ControllerName,
                    StatusCode = statusCode,
                    Request = request,
                    Response = response,
                    ElapsedMilliseconds = timer.ElapsedMilliseconds
                };

                if (exception != null)
                {
                    _logger.LogError(exception, message, log);
                }
                else
                {
                    _logger.LogInformation(message, log);
                }
            }
        }

        /// <summary>
        /// Internal struct that associates a custom log message to a response object.
        /// </summary>
        /// <typeparam name="T">The T.</typeparam>
        protected struct ActionResponse<T>
        {
            /// <summary>
            /// The LogMessage.
            /// </summary>
            public string LogMessage { get; set; }

            /// <summary>
            /// The ResponseObject.
            /// </summary>
            public T ResponseObject { get; set; }
        }
    }
}
