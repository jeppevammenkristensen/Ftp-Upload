using System;
using System.IO;
using ConfigR;

namespace Upload.Configuration
{
    public class ConfigRConfigurationManager : IConfigurationManager
    {
        public static string ConfigurationFolder { get; set; }

        public void Initalize(string[] args)
        {
            LoadScriptFileCreateIfNecesarry();
        }

        private void LoadScriptFileCreateIfNecesarry()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var ftpUploader = Path.Combine(path, "FtpUploader");
            if (!Directory.Exists(ftpUploader))
                Directory.CreateDirectory(ftpUploader);
            var filePath = Path.Combine(ftpUploader, "Config.csx");
            
            if (!File.Exists(filePath))
                File.Copy("Config.csx", filePath);

            Config.Global.LoadScriptFile(filePath, AppDomain.CurrentDomain.GetAssemblies());
        }
    }

    public interface IConfigurationManager
    {
        void Initalize(string[] args);
    }
}