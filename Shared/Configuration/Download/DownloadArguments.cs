namespace Shared.Configuration.Download
{
    public class DownloadArguments
    {
        public string UserName { get; set; }
        public string HostName { get; set; }

        public string EncryptedPassword { get; set; }

        public string FtpPath { get; set; }
        public string DestinationPath { get; set; }
    }
}