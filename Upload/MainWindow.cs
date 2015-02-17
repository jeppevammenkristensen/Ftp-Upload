using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Upload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel Scope { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        protected async override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Scope = new MainWindowViewModel();

            Scope.Location = GlobalParameters.Path;
            await Scope.CheckConfiguration();

            DataContext = Scope;

            if (Scope.IsValid && Scope.NamedConfigurations.Count == 1)
            {
                await Scope.UploadAsync();
                Scope.Status = "Færdig...";
            }
        }
    }
}
