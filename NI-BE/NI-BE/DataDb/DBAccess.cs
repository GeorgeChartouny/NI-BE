using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace NI_BE.DataDb
{
    public class DBAccess
    {
        private readonly IConfiguration _configuration;

        public DBAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<T>> LoadData<T,U>(string storedProcedure, U parameters, string connectionId = "DefaultConnection")
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));

            return await connection.QueryAsync<T>(storedProcedure,parameters,commandType: CommandType.StoredProcedure);

        }
    }
}
