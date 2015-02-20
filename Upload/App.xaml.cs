using System.IO;
using System.Linq;
using System.Windows;
using Upload.Configuration;
using Upload.Configuration.ConfigR;

namespace Upload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            GlobalParameters.Path = e.Args.FirstOrDefault() ?? @"c:\temp\testfolder";
            UploadConfig.SetConfigurationManager(new ConfigRConfigurationManager(), e.Args);
        }
    }

    public class GlobalParameters
    {
        public static string Path { get; set; }
    }
}
