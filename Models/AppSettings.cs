
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CrosshairApp.Models
{
    public class AppSettings : INotifyPropertyChanged
    {
        private bool _launchAtStartup = false;
        private bool _minimizeToTrayOnClose = true;

        public bool LaunchAtStartup
        {
            get => _launchAtStartup;
            set => SetProperty(ref _launchAtStartup, value);
        }

        public bool MinimizeToTrayOnClose
        {
            get => _minimizeToTrayOnClose;
            set => SetProperty(ref _minimizeToTrayOnClose, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
