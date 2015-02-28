using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigR;
using Shared;
using Shared.Configuration;
using Shared.Configuration.Download;
using Shared.Configuration.Json;
using Shared.Infrastructure.Encryption;
using Shared.Infrastructure.Ftp;
using WinSCP;

namespace Download
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurator = new JsonConfigurator();
            var result = configurator.LoadConfiguration<EncryptionConfiguration>(ConfigurationKeys.Encryption);
            var arguments = configurator.LoadConfiguration<DownloadArguments>(ConfigurationKeys.DownloadArguments);
            // This little stunt is done so that Encryption will load this correctly
            Config.Global.Add(ConfigurationKeys.Encryption,result);

            var ftpService = new FtpService();
            var operation = ftpService.GetDownloadOperation(arguments.UserName, arguments.EncryptedPassword.Decrypt(),
                arguments.HostName, arguments.FtpPath, arguments.DestinationPath, TransferCallback);

            operation.StartDownload();
            

            //ftpDownload.Password = arguments.EncryptedPassword;
            //                 Password = password,
            //                 HostName = hostName,


        }

        private static void TransferCallback(TransferEventArgs obj)
        {
            Console.WriteLine(obj.FileName);
        }
    }

    
    
}
