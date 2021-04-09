using System.Data;

namespace Blueprint.HostedService.Dapper
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();

        IDbConnection GetNewConnection();
    }
}
