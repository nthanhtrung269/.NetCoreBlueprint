using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.SharedKernel;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.EventHandlers
{
    public class PreGenerateResizedImagesEvent : DomainEvent
    {
        public PreGenerateResizedImagesEvent(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}
