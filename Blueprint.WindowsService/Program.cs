using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Blueprint.WindowsService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Serilog: https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/
            // Config: https://github.com/serilog/serilog-sinks-rollingfile

            //Read Configuration from appSettings
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

            try
            {
                Log.Information("Blueprint.WindowsService started");
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error in Blueprint.WindowsService");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddLogging(config =>
                    {
                        // Enable the Serilog provider under the default Microsoft.Extensions.Logging implementation
                        // Setting dispose to true will cause Serilog to flush buffer when provider is disposed
                        // https://github.com/serilog/serilog-extensions-logging
                        config.AddSerilog(dispose: true);
                    });
                })
                .UseSerilog();
    }
}
