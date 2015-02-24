using System;
using System.Threading.Tasks;
using DapperExtensions;
using Upload.Annotations;
using Upload.Configuration;
using Upload.Infrastructure.Database;

namespace Upload.Commands
{
    public class UpdateFtpConfigurationCommand
    {
        private ConnectionManager _connectionManager = new ConnectionManager();

        public async Task ExecuteCommand([NotNull] FtpInformation information)
        {
            if (information == null) throw new ArgumentNullException("information");
            using (var connection = await _connectionManager.OpenConnectionAsync())
            {
                connection.Update(information);
            }
        }
    }
}