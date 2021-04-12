using System.Data;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();

        IDbConnection GetNewConnection();
    }
}