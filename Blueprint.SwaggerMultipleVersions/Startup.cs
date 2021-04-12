using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Blueprint.Swagger
{
    public class Startup
    {
        private readonly ILogger _logger;

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

            services.AddLogging(
               builder =>
               {
                   builder.AddFilter("Microsoft", LogLevel.Warning)
                          .AddFilter("System", LogLevel.Warning)
                          .AddFilter("NToastNotify", LogLevel.Warning)
                          .AddConsole();
               });

            services.AddApiVersioning(x =>
            {
                x.DefaultApiVersion = new ApiVersion(1, 0);
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.ReportApiVersions = true;
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new OpenApiInfo
                {
                    Version = "v1.0",
                    Title = "Asset API",
                    Description = "An Asset ASP.NET Core Web API",
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

                c.SwaggerDoc("v2.0", new OpenApiInfo
                {
                    Version = "v2.0",
                    Title = "Asset API v2.0",
                    Description = "A Asset API V2.0 ASP.NET Core Web API",
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

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo))
                    {
                        return false;
                    }

                    IEnumerable<ApiVersion> versions = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    if (!versions.Any()) // BYPASS action w/o ApiVersion attribute
                    {
                        return true;
                    }

                    return versions.Any(v => $"v{v}" == docName);
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            // https://localhost:44312/index.html
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1.0/swagger.json", "Asset API v1.0");
                c.SwaggerEndpoint("./swagger/v2.0/swagger.json", "Asset API v2.0");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
