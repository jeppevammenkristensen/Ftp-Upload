using System.ComponentModel;
using System.Runtime.CompilerServices;
using Upload.Annotations;

namespace Upload
{
    public class Status : INotifyPropertyChanged
    {
        private string _path;
        private int _progress;

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

        public int Progress
        {
            get { return _progress; }
            set
            {
                if (value.Equals(_progress)) return;
                _progress = value;
                OnPropertyChanged();
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