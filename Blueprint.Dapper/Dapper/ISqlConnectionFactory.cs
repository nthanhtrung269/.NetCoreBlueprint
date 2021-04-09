using System.Data;

namespace Blueprint.Dapper.Dapper
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();

        IDbConnection GetNewConnection();
    }
}
