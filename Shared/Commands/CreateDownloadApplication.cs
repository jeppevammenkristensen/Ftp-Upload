using System.IO;
using System.Threading.Tasks;
using ConfigR;
using Shared.Configuration;
using Shared.Configuration.Download;
using Shared.Configuration.Json;
using Shared.Infrastructure.Files;

namespace Shared.Commands
{
    public class CreateDownloadApplication
    {
        public string FolderName { get; set; }
        public string EncryptedPassword { get; set; }
        public string FtpPath { get; set; }

        public string HostName { get; set; }
        public string UserName { get; set; }

        public async Task CreateDownloader()
        {
            var encryption = Config.Global.Get<EncryptionConfiguration>();
            var downloadConfiguration = Config.Global.Get<DownloadApplication>();
            var jsonConfigurator = new JsonConfigurator();
            

            var destinationPath = Path.Combine("c:\\", "Temp", FolderName);
            await FileCopier.CopyFromOneFolderToAnother(downloadConfiguration.Location, destinationPath, true);
            jsonConfigurator.SaveConfiguration(ConfigurationKeys.Encryption, encryption, destinationPath);

            var downloadArguments = new DownloadArguments
            {
                DestinationPath = "Extracted",
                EncryptedPassword =  EncryptedPassword,
                FtpPath = FtpPath,
                HostName = HostName,
                UserName = UserName
            };

            jsonConfigurator.SaveConfiguration(ConfigurationKeys.DownloadArguments, downloadArguments, destinationPath);

        }
    }
}