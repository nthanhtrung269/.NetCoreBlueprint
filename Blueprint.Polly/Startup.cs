using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Blueprint.Polly.Models;
using Blueprint.Polly.WeatherForecastApi;
using Blueprint.Polly.WebApi;
using System;
using System.Net;
using System.Security.Authentication;

namespace Blueprint.Polly
{
    public class Startup
    {
        private readonly ILogger _logger;
        private readonly string _environmentName;

        /// <summary>
        /// The Startup constructor.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The environment.</param>
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            // Gets the factory for ILogger instances.
            var nlogLoggerProvider = new NLogLoggerProvider();
            // Creates an ILogger.
            _logger = nlogLoggerProvider.CreateLogger(typeof(Startup).FullName);

            // Gets environment in the web.config file https://weblog.west-wind.com/posts/2020/Jan/14/ASPNET-Core-IIS-InProcess-Hosting-Issues-in-NET-Core-31
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _environmentName = environmentName;
            _logger.LogInformation($"Environment name: {environmentName}");

            var builder = new ConfigurationBuilder()
            .SetBasePath(environment.ContentRootPath)
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName ?? "Production"}.json", optional: true);

            // Sets the new Configuration
            configuration = builder.Build();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddOptions();
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));
            services.Configure<FaultHandlingConfiguration>(Configuration.GetSection("FaultHandling"));

            services.AddScoped<ICachingService, CachingService>();
            services.AddScoped<ITransactionLogService, TransactionLogService>();

            // Use IHttpClientFactory to implement resilient HTTP requests
            // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddSingleton<IWebApiPolicyFactory, WebApiPolicyFactory>();
            services.AddHttpClient<IWeatherForecastApiService, WeatherForecastApiService>().AddDefaultFaultHandlingPolicies();
            services.AddHttpClient<IWeatherForecastApiNotUsingPollyService, WeatherForecastApiNotUsingPollyService>();

            services.AddLogging(
               builder =>
               {
                   builder.AddFilter("Microsoft", LogLevel.Warning)
                          .AddFilter("System", LogLevel.Warning)
                          .AddFilter("NToastNotify", LogLevel.Warning)
                          .AddConsole();
               });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!IsProduction(env))
            {
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandler(CustomExceptionHandlerMiddleware(true));
            }
            else
            {
                app.UseExceptionHandler(CustomExceptionHandlerMiddleware(false));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private bool IsProduction(IWebHostEnvironment env)
        {
            var applicationSettings = Configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();
            bool isProduction = env.IsProduction()
                || applicationSettings.ProductionEnvironment.Equals(_environmentName, StringComparison.OrdinalIgnoreCase);
            return isProduction;
        }

        private Action<IApplicationBuilder> CustomExceptionHandlerMiddleware(bool isDevelopment)
        {
            return applicationBuilder => applicationBuilder.Run(async httpContext =>
            {
                var exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
                Exception specificException = exceptionHandlerPathFeature.Error;
                _logger.LogError("Api Unhandle Exception: {0}", JsonConvert.SerializeObject(specificException));

                var code = HttpStatusCode.InternalServerError;
                var responseObject = new BaseResponseObject
                {
                    Status = false,
                    ErrorCode = ResponseErrorCode.UnhandleException,
                    Message = specificException.Message,
                    StackTrace = isDevelopment ? specificException.StackTrace : string.Empty,
                    Data = specificException.Data
                };

                switch (specificException)
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
                    case BrokenCircuitException _:
                        code = HttpStatusCode.ServiceUnavailable;
                        responseObject.ErrorCode = ResponseErrorCode.ServiceUnavailable;
                        break;
                    case TimeoutRejectedException _:
                        code = HttpStatusCode.RequestTimeout;
                        responseObject.ErrorCode = ResponseErrorCode.RequestTimeout;
                        break;
                    case WebApiException _:
                        code = HttpStatusCode.ServiceUnavailable;
                        responseObject.ErrorCode = ResponseErrorCode.ExternalServiceUnavailable;
                        break;
                }

                var result = JsonConvert.SerializeObject(responseObject);
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)code;

                await httpContext.Response.WriteAsync(result);
            }
            );
        }
    }
}
