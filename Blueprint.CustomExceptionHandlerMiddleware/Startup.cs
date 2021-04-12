using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Blueprint.CustomExceptionHandlerMiddleware.Project.BusinessServices;
using Blueprint.CustomExceptionHandlerMiddleware.Project.DataModels;
using Blueprint.CustomExceptionHandlerMiddleware.Project.Models;
using System;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project
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

            // Dependency injection support for Mapster
            // https://github.com/MapsterMapper/Mapster/wiki/Dependency-Injection
            var config = new TypeAdapterConfig();
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            services.AddScoped<ICachingWorkerService, CachingWorkerService>();
            services.AddScoped<ITransactionLogService, TransactionLogService>();
            AddMapperConfigurations(config);

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceScopeFactory serviceScopeFactory)
        {
            if (!IsProduction(env))
            {
                app.UseDeveloperExceptionPage();
                app.UseCustomExceptionHandlerMiddleware();
            }
            else
            {
                app.UseCustomExceptionHandlerMiddleware();
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
            return env.IsProduction();
        }

        private void AddMapperConfigurations(TypeAdapterConfig config)
        {
            config.NewConfig<WeatherForecastDataModel, WeatherForecast>();
        }
    }
}
