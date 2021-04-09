using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Blueprint.BaseApiController.Controllers
{
    /// <summary>
    /// Base class for ApiController derived classes.
    /// </summary>
    public class BaseApiController : ControllerBase
    {
        private readonly ILogger<BaseApiController> _logger;

        private string ControllerName { get; }
        private string InvalidRequestLogMessage => $"{ControllerName} request is invalid";
        private string RequestCompletedLogMessage => $"{ControllerName} successful";

        /// <summary>
        /// Constructor for BaseApiControllers.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public BaseApiController(ILogger<BaseApiController> logger)
        {
            _logger = logger;

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
            ValidateRequestWithGuard(request);
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
        protected async Task<TResponse> ValidateAndProcessRequestWithGuardAsync<TRequest, TResponse>(TRequest request, Func<TRequest, Task<TResponse>> executeRequest)
        {
            ValidateRequestWithGuard(request);
            return await ProcessRequestAsync(request, executeRequest);
        }

        private TResponse ValidateRequest<TRequest, TResponse>(TRequest request)
            where TResponse : BaseResponseObject, new()
        {
            if (request == null)
            {
                _logger.LogWarning(InvalidRequestLogMessage, new
                {
                    Request = Request.Body
                });

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
            Guard.AgainstNull(nameof(TRequest), request);
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
            _logger.LogDebug($"{ControllerName} controller", request);

            try
            {
                ActionResponse<TResponse> executionResult = executeRequest(request);

                _logger.LogInformation(executionResult.LogMessage, new
                {
                    Request = request,
                    Response = executionResult.ResponseObject
                });

                return executionResult.ResponseObject;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ControllerName} encountered an unexpected error", ex, new
                {
                    Request = request
                });

                throw;
            }
        }

        private async Task<TResponse> ProcessRequestAsync<TRequest, TResponse>(TRequest request, Func<TRequest, Task<TResponse>> executeRequest)
        {
            return await ProcessRequestAsync(request, async req =>
            {
                TResponse response = await executeRequest(req);

                return new ActionResponse<TResponse>
                {
                    LogMessage = RequestCompletedLogMessage,
                    ResponseObject = response
                };
            });
        }

        private async Task<TResponse> ProcessRequestAsync<TRequest, TResponse>(TRequest request, Func<TRequest, Task<ActionResponse<TResponse>>> executeRequest)
        {
            _logger.LogDebug($"{ControllerName} controller", request);

            try
            {
                ActionResponse<TResponse> executionResult = await executeRequest(request);

                _logger.LogInformation(executionResult.LogMessage, new
                {
                    Request = request,
                    Response = executionResult.ResponseObject
                });

                return executionResult.ResponseObject;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ControllerName} encountered an unexpected error", ex, new
                {
                    Request = request
                });

                throw;
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
