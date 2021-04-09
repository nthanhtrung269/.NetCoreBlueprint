using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Blueprint.Dapper.Dapper
{
    public class DapperRepository : IReadRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public DapperRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<List<T>> QueryAsync<T>(IDbConnection dbConnection, string sql, object param = null) where T : class
        {
            var query = await dbConnection.QueryAsync<T>(sql, param);
            return query.AsList();
        }

        public async Task<List<T>> QueryAsync<T>(string sql, object param = null) where T : class
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            var query = await QueryAsync<T>(connection, sql, param);
            return query.AsList();
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(IDbConnection dbConnection, string sql, object param = null) where T : class
        {
            return await dbConnection.QuerySingleOrDefaultAsync<T>(sql, param);
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null) where T : class
        {
            var connection = _sqlConnectionFactory.GetOpenConnection();
            return await QuerySingleOrDefaultAsync<T>(connection, sql, param);
        }
    }
}