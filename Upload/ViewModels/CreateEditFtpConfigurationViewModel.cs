using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Upload.Annotations;
using Upload.Infrastructure.Ftp;

namespace Upload.ViewModels
{
    public class CreateEditFtpConfigurationViewModel : INotifyPropertyChanged
    {
        private Lazy<FtpService> _ftpService = new Lazy<FtpService>();

        private string _server;
        private string _userName;
        private string _password;
        private string _testResult;

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

        public string Password
        {
            get { return _password; }
            set
            {
                if (value == _password) return;
                _password = value;
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

        public async void TestConnection()
        {
            var result = await _ftpService.Value.CheckConnectionAsync(UserName, Password, Server);
            if (result == FtpService.FtpConnectionResult.Valid)
                TestResult = "Valid forbindelse";
            else
            {
                TestResult = "Kunne ikke forbinde";
            }

        }

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