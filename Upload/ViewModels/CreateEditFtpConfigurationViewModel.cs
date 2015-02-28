using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Shared.Commands;
using Shared.Configuration;
using Shared.Infrastructure.Encryption;
using Shared.Infrastructure.Ftp;
using Upload.Annotations;

namespace Upload.ViewModels
{
    public class CreateEditFtpConfigurationViewModel : INotifyPropertyChanged, IDataErrorInfo
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

        [Required]
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

        [Required]
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

        [Required]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                if (value == _path) return;
                _path = value;
                OnPropertyChanged();
                
            }
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
            {
                TestResult = "Valid forbindelse";
                Connection.Clear();
            }
            else
            {
                TestResult = "Kunne ikke forbinde";
                Connection.Add("Server", "Kunne ikke forbinde");
                Connection.Add("UserName", "Kunne ikke forbinde");
            }

            OnPropertyChanged("Server");
            OnPropertyChanged("UserName");
        }

        public async Task<bool> Save()
        {
            await TestConnection();

            var isValid = await ValidateAll();
            if (!isValid)
                return false;

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
            return true;
        }

        private async Task<bool> ValidateAll()
        {
            var properties = new[]
            {
                "Server", "Name", "Path",
                "UserName"
            };

            foreach (var property in properties)
            {
                OnPropertyChanged(property);
            }

            return properties.All(x => string.IsNullOrWhiteSpace(GetErrorMessageForProperty(x)));
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


        protected string ValidateProperty(object value = null,string propertyName = null)
        {
            if (propertyName == null)
                return string.Empty;

            ValidationContext context = new ValidationContext(this, null, null);
            context.MemberName = propertyName;

            ICollection<ValidationResult> results = new List<ValidationResult>();
            var validate = Validator.TryValidateProperty(value, context,results);
            if (!validate)
                return string.Join("-", results.Select(x => x.ErrorMessage));
            return string.Empty;

        }

         string IDataErrorInfo.this[string propertyName]
        {
            get {
                return GetErrorMessageForProperty(propertyName);
            }
        }

         private string GetErrorMessageForProperty(string propertyName)
         {
             if (Connection.ContainsKey(propertyName))
                 return Connection[propertyName];

             return ValidateProperty(this.GetType().GetProperty(propertyName).GetValue(this, null), propertyName);
         }

        public string Error
        {
            get { return string.Empty; }
        }

        private static readonly IDictionary<string,string> Connection = new Dictionary<string, string>();
    }    
}
