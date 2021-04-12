using System.Threading.Tasks;

namespace Blueprint.EntityFrameworkRepositoryAsync
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
