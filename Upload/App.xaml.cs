﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using ConfigR;
using Shared.Configuration.Database;

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
            //UploadConfig.SetConfigurationManager(new ConfigRConfigurationManager(), e.Args);
            
            var bootstrapper = new Bootstrapper();
            bootstrapper.Bootstrap();
        }
        
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "No cigar!!!");

            // Prevent default unhandled exception processing
            e.Handled = true;
        }
    }
    

    public class GlobalParameters
    {
        public static string Path { get; set; }
    }
}
