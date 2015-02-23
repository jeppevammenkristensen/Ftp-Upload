using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Upload.ViewModels;

namespace Upload
{
    /// <summary>
    /// Interaction logic for CreateEditFtpConfigurationView.xaml
    /// </summary>
    public partial class CreateEditFtpConfigurationView : Window
    {
        public CreateEditFtpConfigurationViewModel Scope { get; set; }
        public MainWindowViewModel MainViewModel { get; set; }

        public CreateEditFtpConfigurationView()
        {
            InitializeComponent();

        }

        protected async override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Scope = new CreateEditFtpConfigurationViewModel();
            Scope.MainViewModel = MainViewModel;
            Scope.GetText = () => PasswordTextBox.Password;
            DataContext = Scope;
        }


        private async void Test_Click(object sender, RoutedEventArgs e)
        {
            await Scope.TestConnection();
        }


        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            await Scope.Save();
            this.Close();

        }
    }
}
