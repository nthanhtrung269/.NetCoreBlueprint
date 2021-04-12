using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces
{
    public interface ICachingService
    {
        Task<bool> IsLoggingDatabaseAsync();
    }
}
