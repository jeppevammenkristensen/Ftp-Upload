using System;
using System.IO;
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

        public FtpDownloadOperation GetDownloadOperation(string userName, string password, string hostName, string ftpPath, string destination, Action<TransferEventArgs> callback)
        {
            var ftpDownloadOperation = new FtpDownloadOperation();
            ftpDownloadOperation.UserName = userName;
            ftpDownloadOperation.Password = password;
            ftpDownloadOperation.HostName = hostName;

            ftpDownloadOperation.FtpPath = ftpPath;
            ftpDownloadOperation.DestinationPath = destination;

            ftpDownloadOperation.UpdateCallBack = callback;

            return ftpDownloadOperation;

        }

        public class FtpDownloadOperation
        {
            public async Task StartDownloadAsync()
            {
               await  Task.Run(() =>
               {
                  StartDownload();
               });
            }

            public void StartDownload()
            {
                Directory.CreateDirectory(DestinationPath);

                try
                {
                    // Setup session options
                    SessionOptions sessionOptions = new SessionOptions
                    {
                        Protocol = Protocol.Ftp,
                        HostName = HostName,
                        UserName = UserName,
                        Password = Password,
                        //SshHostKeyFingerprint = "ssh-rsa 2048 xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx:xx"
                    };

                    using (Session session = new Session())
                    {
                        // Will continuously report progress of synchronization
                        session.FileTransferred += FileTransfered;

                        // Connect
                        session.Open(sessionOptions);

                        // Synchronize files
                        SynchronizationResult synchronizationResult;
                        synchronizationResult =
                            session.SynchronizeDirectories(
                                SynchronizationMode.Local, DestinationPath, FtpPath, false);

                        // Throw on any error
                        synchronizationResult.Check();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.ToString());
                }
            }

            private void FileTransfered(object sender, TransferEventArgs e)
            {
                UpdateCallBack(e);
            }

            public string UserName { get; set; }
            public string Password { get; set; }
            public string HostName { get; set; }
            public string FtpPath { get; set; }
            public string DestinationPath { get; set; }
            public Action<TransferEventArgs> UpdateCallBack { get; set; }
        }
    }
}