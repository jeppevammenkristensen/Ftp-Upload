using System;
using System.Threading.Tasks;
using DapperExtensions;
using Shared.Configuration;
using Shared.Infrastructure.Database;

namespace Shared.Commands
{
    public class UpdateFtpConfigurationCommand
    {
        private ConnectionManager _connectionManager = new ConnectionManager();

        public async Task ExecuteCommand(FtpInformation information)
        {
            if (information == null) throw new ArgumentNullException("information");
            using (var connection = await _connectionManager.OpenConnectionAsync())
            {
                connection.Update(information);
            }
        }
    }
}