using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Download
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = new Arguments();

            var ftpDownload = new FtpDownload();
            ftpDownload.UserName = arguments.UserName;
            ftpDownload.HostName = arguments.HostName;
            //ftpDownload.Password = arguments.EncryptedPassword;
            //                 Password = password,
            //                 HostName = hostName,


        }
    }

    public class Arguments
    {
        public string UserName { get; set; }
        public string HostName { get; set; }
    }
    
}
