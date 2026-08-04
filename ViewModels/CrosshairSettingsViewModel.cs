
using CommunityToolkit.Mvvm.ComponentModel;
using CrosshairApp.Models;

namespace CrosshairApp.ViewModels
{
    public partial class CrosshairSettingsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private CrosshairSettings _settings;

        public CrosshairSettingsViewModel(CrosshairSettings settings)
        {
            _settings = settings;
        }

        public CrosshairSettingsViewModel() : this(new CrosshairSettings()) { }
    }
}
