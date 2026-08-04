
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrosshairApp.Models;
using CrosshairApp.Services;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace CrosshairApp.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private CrosshairSettings _currentCrosshairSettings = new CrosshairSettings();

        [ObservableProperty]
        private ViewModelBase _currentPage = null!;

        public CrosshairSettingsViewModel CrosshairSettingsVm { get; private set; } = null!;
        public SettingsViewModel SettingsVm { get; private set; } = null!;

        [ObservableProperty]
        private AppSettings _appSettings = new AppSettings();

        private readonly IOverlayService _overlayService;
        private readonly ISettingsService _settingsService;

        public MainViewModel(IOverlayService overlayService, ISettingsService settingsService)
        {
            _overlayService = overlayService;
            _settingsService = settingsService;

            CrosshairSettingsVm = new CrosshairSettingsViewModel(CurrentCrosshairSettings);
            SettingsVm = new SettingsViewModel(_settingsService, AppSettings);
            CurrentPage = CrosshairSettingsVm;

            LoadInitialData();
        }

        private async void LoadInitialData()
        {
            AppSettings = await _settingsService.LoadSettingsAsync();
            SettingsVm = new SettingsViewModel(_settingsService, AppSettings);

            // Show overlay on startup
            _overlayService.ShowOverlay(CurrentCrosshairSettings);
        }

        [RelayCommand]
        private void Navigate(string page)
        {
            CurrentPage = page switch
            {
                "Settings" => SettingsVm,
                _ => CrosshairSettingsVm,
            };
        }

        [RelayCommand]
        private void ToggleOverlay()
        {
            if (_overlayService.IsOverlayVisible)
                _overlayService.HideOverlay();
            else
                _overlayService.ShowOverlay(CurrentCrosshairSettings);
        }

        partial void OnCurrentCrosshairSettingsChanged(CrosshairSettings value)
        {
            _overlayService.UpdateOverlaySettings(value);
        }
    }
}
