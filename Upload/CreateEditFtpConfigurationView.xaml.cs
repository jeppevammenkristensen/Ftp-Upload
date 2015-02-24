using System;
using System.Threading.Tasks;
using System.Windows;
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
        public int? Id { get; set; }

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
            Scope.SetText = (password) => PasswordTextBox.Password = password;
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

        public async Task InjectValues(MainWindowViewModel scope, int? id = null)
        {
            Scope.MainViewModel = scope;
            Scope.Id = id;
            await Scope.LoadAsync();
        }
    }
}
