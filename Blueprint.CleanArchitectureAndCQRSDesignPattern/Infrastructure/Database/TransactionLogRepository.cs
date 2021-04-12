using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure.Database
{
    public class TransactionLogRepository : EfRepository<DBContext, BlueprintTransactionLog, long>, ITransactionLogRepository
    {
        public TransactionLogRepository(DBContext dbContext) : base(dbContext)
        {
        }
    }
}