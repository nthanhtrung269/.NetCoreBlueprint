using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Blueprint.HostedService.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.HostedService.Services
{
    public class FileProcessingHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly Timer _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly AssetSettings _configuration;
        private bool _disposed = false;

        public FileProcessingHostedService(ILogger<FileProcessingHostedService> logger,
            IOptions<AssetSettings> config,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _configuration = config.Value;
            _serviceScopeFactory = serviceScopeFactory;
            _timer = new Timer(callback: async _ => await DoWorkAsync());
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(FileProcessingHostedService)} Background Service is starting.");
            _logger.LogInformation($"{nameof(FileProcessingHostedService)} TimeSpan in seconds: {_configuration.FileProcessingHostedServiceTimeSpanInSeconds}.");
            _timer.Change(dueTime: TimeSpan.Zero, period: TimeSpan.FromSeconds(_configuration.FileProcessingHostedServiceTimeSpanInSeconds));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(FileProcessingHostedService)} Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _timer?.Dispose();
            }

            _disposed = true;
        }

        private async Task DoWorkAsync()
        {
            _logger.LogInformation($"{nameof(FileProcessingHostedService)} Background Service is working.");

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    IFileService fileService = scope.ServiceProvider.GetService<IFileService>();
                    var file = await fileService.GetFileByIdQuery(1);

                    if (file != null)
                    {
                        _logger.LogInformation($"Processing file id: {1}, cloud url: {file.CloudUrl}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Call to {nameof(FileProcessingHostedService)} -- {nameof(DoWorkAsync)} failed.");
            }
        }
    }
}
