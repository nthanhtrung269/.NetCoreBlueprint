using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Blueprint.HostedService.Dapper
{
    public interface IReadRepository
    {
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null) where T : class;
        Task<T> QuerySingleOrDefaultAsync<T>(IDbConnection dbConnection, string sql, object param = null) where T : class;
        Task<List<T>> QueryAsync<T>(string sql, object param = null) where T : class;
        Task<List<T>> QueryAsync<T>(IDbConnection dbConnection, string sql, object param = null) where T : class;
    }
}
