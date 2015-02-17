using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ConfigR;

namespace Upload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            GlobalParameters.Path = e.Args.FirstOrDefault() ?? Directory.GetCurrentDirectory();
            LoadScriptFileCreateIfNecesarry();
        }

        private void LoadScriptFileCreateIfNecesarry()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var ftpUploader = Path.Combine(path, "FtpUploader");
            if (!Directory.Exists(ftpUploader))
                Directory.CreateDirectory(ftpUploader);
            var filePath = Path.Combine(ftpUploader, "Config.csx");
            GlobalParameters.ConfigurationFolder = ftpUploader;
            if (!File.Exists(filePath))
                File.Copy("Config.csx", filePath);

            Config.Global.LoadScriptFile(filePath, AppDomain.CurrentDomain.GetAssemblies());
        }
    }

    public class GlobalParameters
    {
        public static string Path { get; set; }

        public static string ConfigurationFolder { get; set; }
    }
}
