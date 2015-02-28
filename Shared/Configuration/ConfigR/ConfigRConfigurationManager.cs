using System;
using System.IO;
using ConfigR;

namespace Shared.Configuration.ConfigR
{
    public class ConfigRConfigurationManager : IInitializableConfigurationManager
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

        public T Get<T>(string key)
        {
            return Config.Global.Get<T>(key);
        }
    }

    public interface IInitializableConfigurationManager : IConfigurationManager
    {
        void Initalize(string[] args);
    }

    public interface IConfigurationManager
    {
        T Get<T>(string key);
    }
}