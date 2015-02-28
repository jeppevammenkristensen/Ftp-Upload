using System;
using System.Threading.Tasks;
using WinSCP;

namespace Shared.Infrastructure.Ftp
{
    public class FtpService
    {
        public enum FtpConnectionResult
        {
            Valid,
            ConnectionOptionsWrong
        }

        public async Task<FtpConnectionResult> CheckConnectionAsync(string userName, string password, string hostName)
        {
             return await Task.Run(() =>
             {

                 using (var session = new Session())
                 {


                     try
                     {
                         session.Open(new SessionOptions()
                         {
                             Protocol = Protocol.Ftp,
                             UserName = userName,
                             Password = password,
                             HostName = hostName,
                         });

                         return FtpConnectionResult.Valid;
                     }
                     catch (Exception )
                     {
                         return FtpConnectionResult.ConnectionOptionsWrong;
                     }
                 }
             });
        }
    }
}