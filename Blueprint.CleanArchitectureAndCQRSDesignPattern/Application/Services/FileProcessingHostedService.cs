using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.EventHandlers;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Enums;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Services
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
                    IDomainEventService _domainEventService = scope.ServiceProvider.GetService<IDomainEventService>();
                    IFileRepository fileRepository = scope.ServiceProvider.GetService<IFileRepository>();
                    List<string> supportFiles = _configuration.FileTypes;
                    List<string> supportedCategoryTypes = _configuration.SupportedCategoryTypes;
                    List<BlueprintFile> blueprintFiles = fileRepository.GetFilesForBackgroudProcessing(supportFiles, supportedCategoryTypes, _configuration.FileProcessingHostedServiceTotalFilesProcessedPerTimes).ToList();

                    try
                    {
                        foreach (var blueprintFile in blueprintFiles)
                        {
                            await _domainEventService.Publish(new PreGenerateResizedImagesEvent(blueprintFile.Id));
                        }

                        fileRepository.UpdateRangeForBackgroudProcessing(blueprintFiles, (int)BackgroudProcessingStatus.Processed);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Call to {nameof(PreGenerateResizedImagesEvent)} failed.");
                        fileRepository.UpdateRangeForBackgroudProcessing(blueprintFiles, (int)BackgroudProcessingStatus.Failed);
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
