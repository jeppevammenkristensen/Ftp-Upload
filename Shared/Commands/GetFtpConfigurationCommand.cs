using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Shared.Configuration;
using Shared.Infrastructure.Database;

namespace Shared.Commands
{
    public class GetFtpConfigurationCommand
    {
        private ConnectionManager _manager = new ConnectionManager();

        public async Task<FtpInformation> ExecuteAsync(int id)
        {
            using (var connection = await _manager.OpenConnectionAsync())
            {
                var result = await connection.QueryAsync<FtpInformation>("SELECT * From FtpInformation Where Id = @Id", new {id});
                return result.FirstOrDefault();
            }
        }
        
    }
}