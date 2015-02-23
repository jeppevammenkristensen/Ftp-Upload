using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Upload.Configuration;
using Upload.Infrastructure.Database;

namespace Upload.Commands
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