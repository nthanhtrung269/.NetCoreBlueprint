using System.Threading.Tasks;

namespace Blueprint.HttpClient1.BusinessServices
{
    public interface IAggregateService
    {
        Task<bool> ImportOffers();
    }
}
