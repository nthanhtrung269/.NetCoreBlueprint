using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Services;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Enums;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Exceptions;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.SharedKernel;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure;
using StackifyLib;
using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Security.Authentication;
using System.Text;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern
{
    /// <summary>
    /// The Startup class.
    /// This project is built based on Clean Architecture https://github.com/ardalis/CleanArchitecture.
    /// And based on DDD: https://fildev.net/category/design-pattern/domain-driven-design/
    /// Document: https://github.com/dotnet-architecture/eShopOnWeb
    /// </summary>
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
            .AddJsonFile($"appsettings.{environmentName ?? "Production"}.json", optional: true)
            .AddJsonFile("appsettings.buildnote.json", optional: false, reloadOnChange: true);

            // Sets the new Configuration
            configuration = builder.Build();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApplication();
            services.AddInfrastructure(Configuration);
            services.AddHostedService<FileProcessingHostedService>();

            services.AddLogging(
               builder =>
               {
                   builder.AddFilter("Microsoft", LogLevel.Warning)
                          .AddFilter("System", LogLevel.Warning)
                          .AddFilter("NToastNotify", LogLevel.Warning)
                          .AddConsole();
               });

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddMvc().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            SetupJWTServices(services);

            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var result = new BadRequestObjectResult(context.ModelState);

                        result.ContentTypes.Add(MediaTypeNames.Application.Json);
                        result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                        return result;
                    };
                });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Asset API",
                    Description = "A Asset ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Blueprint",
                        Email = string.Empty
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use Blueprint licence"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="env">The host environment.</param>
        /// <param name="serviceScopeFactory">The serviceScopeFactory.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceScopeFactory serviceScopeFactory)
        {
            app.ConfigureStackifyLogging(Configuration); //This is critical!!
            app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();

            if (!IsProductionEnvironment(env))
            {
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandler(CustomExceptionHandlerMiddleware(serviceScopeFactory, true));
            }
            else
            {
                app.UseExceptionHandler(CustomExceptionHandlerMiddleware(serviceScopeFactory, false));
            }

            app.UseHttpsRedirection();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "Asset API v1");
                c.RoutePrefix = string.Empty;
            });
        }

        private void SetupJWTServices(IServiceCollection services)
        {
            string key = Configuration["Jwt:Key"]; //this should be same which is used while creating token

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });
        }

        private Action<IApplicationBuilder> CustomExceptionHandlerMiddleware(IServiceScopeFactory serviceScopeFactory, bool isDevelopment)
        {
            return applicationBuilder => applicationBuilder.Run(async httpContext =>
            {
                var exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
                Exception specificException = exceptionHandlerPathFeature.Error;
                _logger.LogError("Api Unhandle Exception: {0}", JsonConvert.SerializeObject(specificException));

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    ICachingService cachingService = scope.ServiceProvider.GetService<ICachingService>();
                    ITransactionLogService transactionLogService = scope.ServiceProvider.GetService<ITransactionLogService>();

                    if (await cachingService.IsLoggingDatabaseAsync())
                    {
                        await transactionLogService.AddTransactionLogAsync(TransactionLogStep.CustomExceptionHandlerMiddleware, TransactionLogStatus.Error, JsonConvert.SerializeObject(specificException), string.Empty, specificException.Message, specificException.StackTrace);
                    }
                }

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
                }

                var result = JsonConvert.SerializeObject(responseObject);
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)code;

                await httpContext.Response.WriteAsync(result);
            }
            );
        }

        private bool IsProductionEnvironment(IWebHostEnvironment env)
        {
            return env.IsProduction();
        }
    }
}
