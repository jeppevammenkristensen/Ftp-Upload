using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Upload.Annotations;
using Upload.Configuration;
using Upload.Infrastructure.Encryption;
using Upload.Infrastructure.Ftp;
using WinSCP;

namespace Upload.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Lazy<FtpService>  _ftpService = new Lazy<FtpService>();

        private List<FtpInformation> _configurations;
        private string _location;
        private string _status;
        private ObservableCollection<Status> _fileStatusInformations;
        private bool _isValid;
        private ObservableCollection<string> _missingProperties;
        private int _currentConfigurationIndex;
        private double _overallProgress;
        private int _cps;

        public FtpInformation Configuration { get; set; }

        public MainWindowViewModel()
        {
            FileStatusInformations = new ObservableCollection<Status>();
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
            get { return Configuration != null ? Configuration.Name : null; }
        }

        public ObservableCollection<string> NamedConfigurations { get; set; }

        public bool ShowInvalid
        {
            get { return !IsValid; }
        }

        public int CurrentConfigurationIndex
        {
            get { return _currentConfigurationIndex; }
            set
            {
                if (value == _currentConfigurationIndex) return;
                _currentConfigurationIndex = value;
                OnPropertyChanged();
                CheckConfigurationAsync();

            }
        }

        public ObservableCollection<Status> FileStatusInformations
        {
            get { return _fileStatusInformations; }
            set
            {
                if (Equals(value, _fileStatusInformations)) return;
                _fileStatusInformations = value;
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

        public bool MoreThanOneConfiguration
        {
            get { return _configurations.Count > 1; }
        }


        public async Task UploadAsync()
        {
            Status = "Uploader";
            FileStatusInformations.Clear();

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
                        string.Format("{0}_{1:yyyyMMddHHmmss}", Configuration.Path, DateTime.Now), false,
                        transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    return;

                    //return transferResult.Transfers;
                }
            });

            Status = "Færdig med upload";
        }


        public async Task CheckConfigurationAsync()
        {
            Configuration = _configurations[CurrentConfigurationIndex];
            OnPropertyChanged("MoreThanOneConfiguration");
            OnPropertyChanged("CurrentConfigurationName");
            var missingProperties = Configuration.GetPropertiesMissingInitialization();

            if (missingProperties.Count > 0)
            {
                IsValid = false;
                Status = string.Format("Der mangler følgende properties. {0}", string.Join(",", missingProperties));
                return;
            }

            await CheckConnectionAsync();
            IsValid = true;
        }

        // Temporarily checked out because of it's direct relation to ConfigR
        //public void Open()
        //{
        //    Process.Start("explorer.exe", GlobalParameters.ConfigurationFolder);
        //}

        private void FileTransferProgress(object sender, FileTransferProgressEventArgs e)
        {
            var attr = File.GetAttributes(e.FileName);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var info = FileStatusInformations.FirstOrDefault(x => x.Path == e.FileName);
                if (info == null)
                {
                    info = new Status() {Path = e.FileName};
                    FileStatusInformations.Add(info);
                }

                info.Progress = (int) (e.FileProgress * 100);
            });

            CPS = e.CPS;
            OverallProgress = e.OverallProgress * 100;
        }

        public int CPS
        {
            get { return _cps; }
            set
            {
                if (value == _cps) return;
                _cps = value;
                OnPropertyChanged();
            }
        }

        public double OverallProgress
        {
            get { return _overallProgress; }
            set
            {
                if (value.Equals(_overallProgress)) return;
                _overallProgress = value;
                OnPropertyChanged();
            }
        }

        private SessionOptions GetSessionOptions()
        {
            // Setup session options
            return new SessionOptions
            {
                Protocol = Protocol.Ftp,
                UserName = Configuration.UserName,
                Password = Configuration.Password.Decrypt(),
                HostName = Configuration.Server
            };
        }

        public async Task UpdateConfigurationsAsync()
        {
            await Task.Run(() => {_configurations = UploadConfig.Global.Get<List<FtpInformation>>(ConfigurationKeys.FTP);
            });
            NamedConfigurations = new ObservableCollection<string>(_configurations.Select(x => x.Name));
            CurrentConfigurationIndex = 0;
           
        }

        private async Task CheckConnectionAsync()
        {
            var result = await
                    _ftpService.Value.CheckConnectionAsync(Configuration.UserName, Configuration.Password.Decrypt(),
                        Configuration.Server);

            if (result == FtpService.FtpConnectionResult.Valid)
                Status = "Forbindelse er ok";
            else
            {
                Status = "Forbindelse duer ikke";
            }
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