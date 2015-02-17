using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ConfigR;
using Upload.Annotations;
using Upload.Configuration;
using WinSCP;

namespace Upload
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private FtpInformation _configuration;
        

        public MainWindowViewModel()
        {
            Information = new ObservableCollection<Status>();
        }

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (value.Equals(_isValid)) return;
                _isValid = value;
                OnPropertyChanged();
                OnPropertyChanged("ShowInvalid");
            }
        }

        public bool ShowInvalid
        {
            get { return !IsValid; }
        }

        public ObservableCollection<Status> Information
        {
            get { return _information; }
            set
            {
                if (Equals(value, _information)) return;
                _information = value;
                OnPropertyChanged();
            }
        }


        private string _location;
        private string _status;
        private ObservableCollection<Status> _information;
        private bool _isValid;
        private ObservableCollection<string> _missingProperties;

        public string Status
        {
            get { return _status; }
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        public string Location
        {
            get { return _location; }
            set
            {
                if (value == _location) return;
                _location = value;
                OnPropertyChanged();
            }
        }

        public async Task UploadAsync()
        {
            Status = "Uploader";

            await Task.Run(() =>
            {
                using (Session session = new Session())
                {
                    session.FileTransferProgress += FileTransferProgress;

                    // Connect
                    session.Open(GetSessionOptions());

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult = null;

                    transferResult = session.PutFiles(Location, string.Format("/jvk/CustomUpload_{0:yyyyMMddhhmmss}", DateTime.Now), false, transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    return;

                    //return transferResult.Transfers;
                }
            });
        }

        private void FileTransferProgress(object sender, FileTransferProgressEventArgs e)
        {
            var attr = File.GetAttributes(e.FileName);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return;

            App.Current.Dispatcher.Invoke(() =>
            {
                var info = Information.FirstOrDefault(x => x.Path == e.FileName);
                if (info == null)
                {
                    info = new Status() { Path = e.FileName };
                    Information.Add(info);
                }

                info.Progress = (int)(e.FileProgress * 100);
            });
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task CheckConfiguration()
        {
            _configuration = Config.Global.Get<FtpInformation>("ftp");

            var missingProperties = _configuration.GetPropertiesMissingInitialization();

            if (missingProperties.Count > 0)
            {
                MissingProperties = new ObservableCollection<string>();
                IsValid = false;
                Status = string.Format("Der mangler følgende properties. {0}", string.Join(",", missingProperties));
                return;
            }

            await CheckConnectionAsync();
            IsValid = true;
        }

        public ObservableCollection<string> MissingProperties
        {
            get { return _missingProperties; }
            set
            {
                if (Equals(value, _missingProperties)) return;
                _missingProperties = value;
                OnPropertyChanged();
            }
        }

        public void Open()
        {
            Process.Start("explorer.exe", GlobalParameters.ConfigurationFolder);
        }

        private SessionOptions GetSessionOptions()
        {
            // Setup session options
            return new SessionOptions
            {
                Protocol = Protocol.Ftp,
                UserName = _configuration.UserName,
                Password = _configuration.Password,
                HostName = _configuration.Server
            };
        }

        private async Task CheckConnectionAsync()
        {
            await Task.Run(() =>
            {
                using (var session = new Session())
                {
                    session.Open(GetSessionOptions());
                }
            });
        }
    }
}