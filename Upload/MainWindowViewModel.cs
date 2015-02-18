using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using ConfigR;
using Upload.Annotations;
using Upload.Configuration;
using WinSCP;

namespace Upload
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private List<FtpInformation> _configurations;
        private string _location;
        private string _status;
        private ObservableCollection<Status> _information;
        private bool _isValid;
        private ObservableCollection<string> _missingProperties;

        public FtpInformation Configuration { get; set; }

        public MainWindowViewModel()
        {
            Information = new ObservableCollection<Status>();
            _configurations = Config.Global.Get<List<FtpInformation>>("ftp");
            NamedConfigurations = new ObservableCollection<string>(_configurations.Select(x => x.Name));

            Configuration = _configurations[0];
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

        public string CurrentConfigurationName
        {
            get { return Configuration.Name; }
        }

        public ObservableCollection<string> NamedConfigurations { get; set; }

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

                    transferResult = session.PutFiles(Location,
                        string.Format("{0}_{1:yyyyMMddhhmmss}", Configuration.Path, DateTime.Now), false,
                        transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    return;

                    //return transferResult.Transfers;
                }
            });
        }

        public async Task CheckConfiguration(int index = 0)
        {
            Configuration = _configurations[index];
            OnPropertyChanged("CurrentConfigurationName");
            var missingProperties = Configuration.GetPropertiesMissingInitialization();

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

        private void FileTransferProgress(object sender, FileTransferProgressEventArgs e)
        {
            var attr = File.GetAttributes(e.FileName);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var info = Information.FirstOrDefault(x => x.Path == e.FileName);
                if (info == null)
                {
                    info = new Status() {Path = e.FileName};
                    Information.Add(info);
                }

                info.Progress = (int) (e.FileProgress*100);
            });
        }

        private SessionOptions GetSessionOptions()
        {
            // Setup session options
            return new SessionOptions
            {
                Protocol = Protocol.Ftp,
                UserName = Configuration.UserName,
                Password = Configuration.Password,
                HostName = Configuration.Server
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}