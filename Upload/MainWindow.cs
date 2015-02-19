using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Upload.Messages;
using Upload.ViewModels;

namespace Upload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MessageBus _bus = new MessageBus();

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

            await Scope.UpdateConfigurationsAsync();
            await Scope.CheckConfigurationAsync();

            DataContext = Scope;

            if (Scope.IsValid && Scope.NamedConfigurations.Count == 1)
            {
                await Scope.UploadAsync();
            }
        }

        private async void Upload(object sender, RoutedEventArgs e)
        {
            await Scope.UploadAsync();
        }

        private void Open_Clicked(object sender, RoutedEventArgs e)
        {
            CreateEditFtpConfigurationView view = new CreateEditFtpConfigurationView();
            view.Show();
        }
    }
}
