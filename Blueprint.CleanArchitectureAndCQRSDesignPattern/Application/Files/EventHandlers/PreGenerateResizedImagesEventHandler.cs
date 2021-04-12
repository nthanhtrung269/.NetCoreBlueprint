using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.DomainEvents;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.EventHandlers
{
    class PreGenerateResizedImagesEventHandler : INotificationHandler<DomainEventNotification<PreGenerateResizedImagesEvent>>
    {
        private readonly ILogger<PreGenerateResizedImagesEventHandler> _logger;
        private readonly IPreGeneratorService _preGeneratorService;
        private readonly IFileRepository _fileRepository;

        public PreGenerateResizedImagesEventHandler(ILogger<PreGenerateResizedImagesEventHandler> logger,
            IPreGeneratorService preGeneratorService,
            IFileRepository fileRepository)
        {
            _logger = logger;
            _preGeneratorService = preGeneratorService;
            _fileRepository = fileRepository;
        }

        public async Task Handle(DomainEventNotification<PreGenerateResizedImagesEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            _logger.LogInformation("Handling Domain Event: {DomainEvent}", domainEvent.GetType().Name);

            BlueprintFile blueprintFile = await _fileRepository.GetByIdAsync(domainEvent.Id);

            if (blueprintFile != null)
            {
                await _preGeneratorService.PreGenerateResizedImages(blueprintFile);
            }
        }
    }
}
