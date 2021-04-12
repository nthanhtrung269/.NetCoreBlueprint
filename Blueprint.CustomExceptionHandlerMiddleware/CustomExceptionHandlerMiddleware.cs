using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Newtonsoft.Json;
using Blueprint.CustomExceptionHandlerMiddleware.Project.BusinessServices;
using Blueprint.CustomExceptionHandlerMiddleware.Project.Models;
using System;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ApplicationSettings _settings;

        public CustomExceptionHandlerMiddleware(RequestDelegate next,
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<ApplicationSettings> options)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<CustomExceptionHandlerMiddleware>();
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _serviceScopeFactory = serviceScopeFactory;
            _settings = options.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string requestData = string.Empty;

            try
            {
                requestData = await GetRequestData(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Api Unhandle Exception: {nameof(GetRequestData)}", JsonConvert.SerializeObject(ex));
            }

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError("Api Unhandle Exception", JsonConvert.SerializeObject(ex));

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    ICachingWorkerService cachingService = scope.ServiceProvider.GetService<ICachingWorkerService>();
                    ITransactionLogService transactionLogService = scope.ServiceProvider.GetService<ITransactionLogService>();

                    if (cachingService.IsLoggingDatabase())
                    {
                        await transactionLogService.AddTransactionLogAsync(TransactionLogStep.CustomExceptionHandlerMiddleware, TransactionLogStatus.Error, requestData, ex.Message, JsonConvert.SerializeObject(ex), Guid.Empty);
                    }
                }

                var code = HttpStatusCode.InternalServerError;
                var responseObject = new BaseResponseObject
                {
                    Status = false,
                    ErrorCode = ResponseErrorCode.UnhandleException,
                    Message = ex.Message,
                    StackTrace = !_settings.IsProduction ? ex.StackTrace : string.Empty,
                    Data = ex.Data
                };

                switch (ex)
                {
                    case ArgumentNullException _:
                        code = HttpStatusCode.BadRequest;
                        responseObject.ErrorCode = ResponseErrorCode.ArgumentNullException;
                        break;
                    case ArgumentOutOfRangeException _:
                        code = HttpStatusCode.BadRequest;
                        responseObject.ErrorCode = ResponseErrorCode.ArgumentOutOfRangeException;
                        break;
                    case ArgumentException _:
                        code = HttpStatusCode.BadRequest;
                        responseObject.ErrorCode = ResponseErrorCode.ArgumentException;
                        break;
                    case NotFoundException _:
                        code = HttpStatusCode.NotFound;
                        responseObject.ErrorCode = ResponseErrorCode.NotFound;
                        break;
                    case InvalidOperationException _:
                        code = HttpStatusCode.BadRequest;
                        responseObject.ErrorCode = ResponseErrorCode.InvalidOperationException;
                        break;
                    case AuthenticationException _:
                        code = HttpStatusCode.BadRequest;
                        responseObject.ErrorCode = ResponseErrorCode.AuthenticationException;
                        break;
                }

                var result = JsonConvert.SerializeObject(responseObject);
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)code;

                await httpContext.Response.WriteAsync(result);
            }
        }

        /// <summary>
        /// Gets request data.
        /// Document: https://elanderson.net/2019/12/log-requests-and-responses-in-asp-net-core-3/
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task{System.String}.</returns>
        private async Task<string> GetRequestData(HttpContext context)
        {
            context.Request.EnableBuffering();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            string requestData = $"Http Request Information:{Environment.NewLine}" +
                                   $"Schema:{context.Request.Scheme} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString} " +
                                   $"Request Body: {ReadStreamInChunks(requestStream)}";
            context.Request.Body.Position = 0;

            return requestData;
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }
}
