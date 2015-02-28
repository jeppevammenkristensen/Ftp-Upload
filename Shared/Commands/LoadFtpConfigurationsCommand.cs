using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Shared.Configuration;
using Shared.Infrastructure.Database;

namespace Shared.Commands
{
    public class LoadFtpConfigurationsCommand
    {
        public Lazy<ConnectionManager> ConnectionManager = new Lazy<ConnectionManager>();

        public async Task<List<FtpInformation>> ExecuteAsync()
        {
            using (var connection = await ConnectionManager.Value.OpenConnectionAsync())
            {
                var result = await connection.QueryAsync<FtpInformation>("Select * from FtpInformation");
                return result.ToList();
            }
        }
    }
}