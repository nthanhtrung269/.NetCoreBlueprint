using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.SharedKernel;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database
{
    public interface ITransactionLogRepository : IRepository<BlueprintTransactionLog, long>
    {
    }
}