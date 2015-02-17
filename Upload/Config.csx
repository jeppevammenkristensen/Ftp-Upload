using Upload.Configuration;
using System.Collections.Generic;

var ftpInformations = new List<FtpInformation>();

ftpInformations.Add( new FtpInformation()
{
    Password = "",
    Server =  "",
    UserName = "vertica.dk\\jvk",
    Name = "Bolia",
    Path = "/jvk/bolia" /* The path when connected to the ftp server */,
});

Add("ftp", ftpInformations); 