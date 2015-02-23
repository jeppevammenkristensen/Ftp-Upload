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

Add("encryption", new EncryptionConfiguration(){
	Key = new byte [] {168,202,230,232,184,199,96,32,97,17,192,250,78,23,126,108,226,204,4,224,6,151,172,49,79,33,178,214,142,195,94,86},
	IV = new byte []{207,59,31,207,206,217,104,99,192,34,64,174,195,31,20,242,103,149,175,105,218,220,127,143,93,250,204,22,145,27,81,156}
});

string databaseFile = Upload.Configuration.Utils.ConfigurationPathUtil.CopyEnsuredFile("Data","Configuration.mdf");

Add("configurationConnection", string.Format("Data Source=(LocalDB)\\v11.0;AttachDbFilename=\"{0}\";Integrated Security=True", databaseFile));
