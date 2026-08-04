using CommunityToolkit.Mvvm.ComponentModel;
using CrosshairApp.Models;
using CrosshairApp.Services;

namespace CrosshairApp.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private AppSettings _appSettings;

        public SettingsViewModel(ISettingsService settingsService, AppSettings appSettings)
        {
            _settingsService = settingsService;
            _appSettings = appSettings;
            _appSettings.PropertyChanged += (s, e) =>
            {
                _settingsService.SaveSettingsAsync(_appSettings);
            };
        }

        public SettingsViewModel() : this(new JsonSettingsService(), new AppSettings()) { }
    }
}
