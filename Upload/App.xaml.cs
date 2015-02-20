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
        }
    }

    public class GlobalParameters
    {
        public static string Path { get; set; }
    }
}
