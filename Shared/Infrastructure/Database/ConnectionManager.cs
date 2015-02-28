using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ConfigR;
using Shared.Configuration;

namespace Shared.Infrastructure.Database
{
    public class ConnectionManager
    {
        public async Task<IDbConnection> OpenConnectionAsync()
        {
            var connection = new SqlConnection(Config.Global.Get<string>(ConfigurationKeys.configurationConnection));
            await connection.OpenAsync();
            return connection;
        }
    }
}