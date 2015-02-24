using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Upload.Annotations;
using Upload.Commands;
using Upload.Configuration;
using Upload.Infrastructure.Encryption;
using Upload.Infrastructure.Ftp;

namespace Upload.ViewModels
{
    public class CreateEditFtpConfigurationViewModel : INotifyPropertyChanged
    {
        private Lazy<FtpService> _ftpService = new Lazy<FtpService>();
        private CreateFtpConfigurationCommand _createFtpConfigurationCommand = new CreateFtpConfigurationCommand();
        private UpdateFtpConfigurationCommand _updateFtpConfigurationCommand = new UpdateFtpConfigurationCommand();
        private GetFtpConfigurationCommand _getFtpConfigurationCommand = new GetFtpConfigurationCommand();

        private string _server;
        private string _userName;
        private string _password;
        private string _testResult;

        public int? Id
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }

        public async Task LoadAsync()
        {
            if (Id.HasValue)
            {
                var result = await _getFtpConfigurationCommand.ExecuteAsync(Id.Value);
                Server = result.Server;
                Name = result.Name;
                _password = result.Password.Decrypt();
                SetText(_password);
                UserName = result.UserName;
                Path = result.Path;
            }
        }

        public string Server
        {
            get { return _server; }
            set
            {
                if (value == _server) return;
                _server = value;
                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value == _userName) return;
                _userName = value;
                OnPropertyChanged();
            }
        }

        public string TestResult
        {
            get { return _testResult; }
            set
            {
                if (value == _testResult) return;
                _testResult = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }


        public MainWindowViewModel MainViewModel { get; set; }
        public Action<string> SetText { get; set; }

        public Func<string> GetText;
        private string _name;
        private string _path;
        private int? _id;

        public async Task TestConnection()
        {
            _password = GetText();
            var result = await _ftpService.Value.CheckConnectionAsync(UserName, _password, Server);
            if (result == FtpService.FtpConnectionResult.Valid)
                TestResult = "Valid forbindelse";
            else
            {
                TestResult = "Kunne ikke forbinde";
            }
        }

        public async Task Save()
        {
            if (!Id.HasValue)
            {
                await _createFtpConfigurationCommand.ExecuteCommand(new FtpInformation()
                {
                    Server = Server,
                    Password = _password.Encrypt(),
                    UserName = UserName,
                    Name = Name,
                    Path = Path,
                });
            }
            else
            {
                await _updateFtpConfigurationCommand.ExecuteCommand(new FtpInformation()
                {
                    Id = Id.Value,
                    Server = Server,
                    Password = _password.Encrypt(),
                    UserName = UserName,
                    Name = Name,
                    Path = Path,
                });
            }

            await MainViewModel.UpdateConfigurationsAsync();
        }
        
        // This is temporarily commented out. It was very specific towards configr configuration
        //public void TaskCopyConfiguration()
        //{
        //    var assembly = Assembly.GetExecutingAssembly();
        //    var resourceName = "Upload.Infrastructure.Writer.FtpConfiguration.txt";

        //    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        //    {
        //        using (StreamReader reader = new StreamReader(stream))
        //        {
        //            var format = reader.ReadToEnd();

        //            string result = string.Format(format,_password.Encrypt()
        //                , Server.Stringify()
        //                , UserName.Stringify()
        //                );
        //            Clipboard.SetText(result);
        //            Process.Start("explorer.exe", GlobalParameters.ConfigurationFolder);
        //        }
        //    }
        //}

        #region

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    
}