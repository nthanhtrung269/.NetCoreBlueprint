using MediatR;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.SharedKernel;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.DomainEvents
{
    public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
    {
        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public TDomainEvent DomainEvent { get; }
    }
}
