using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrosshairApp.Models;
using CrosshairApp.Services;

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
        private readonly IHotkeyService _hotkeyService;
        private readonly GameMonitorService _gameMonitorService;
        private readonly IProfileService _profileService;
        private readonly IPresetService _presetService;

        public MainViewModel(
            IOverlayService overlayService,
            ISettingsService settingsService,
            IHotkeyService hotkeyService,
            GameMonitorService gameMonitorService,
            IProfileService profileService,
            IPresetService presetService)
        {
            _overlayService = overlayService;
            _settingsService = settingsService;
            _hotkeyService = hotkeyService;
            _gameMonitorService = gameMonitorService;
            _profileService = profileService;
            _presetService = presetService;

            CrosshairSettingsVm = new CrosshairSettingsViewModel(CurrentCrosshairSettings);
            SettingsVm = new SettingsViewModel(_settingsService, AppSettings);
            CurrentPage = CrosshairSettingsVm;

            LoadInitialData();
        }

        public MainViewModel(IOverlayService overlayService, ISettingsService settingsService)
            : this(overlayService, settingsService, new WindowsHotkeyService(), new GameMonitorService(), new JsonProfileService(), new JsonPresetService())
        {
        }

        private async void LoadInitialData()
        {
            AppSettings = await _settingsService.LoadSettingsAsync();
            SettingsVm = new SettingsViewModel(_settingsService, AppSettings);

            // Show overlay on startup
            _overlayService.ShowOverlay(CurrentCrosshairSettings);

            // Register Win32 Global Hotkeys
            RegisterGlobalHotkeys();

            // Start Game Monitor Service
            var profiles = await _profileService.LoadProfilesAsync();
            var presets = await _presetService.LoadPresetsAsync();
            _gameMonitorService.ProfileDetected += OnGameProfileDetected;
            _gameMonitorService.Start(profiles, presets);
        }

        private void RegisterGlobalHotkeys()
        {
            try
            {
                // F8: Toggle overlay on/off
                _hotkeyService.RegisterHotkey(new KeyGesture(Key.F8), ToggleOverlay);

                // Insert: Hide/Show crosshair
                _hotkeyService.RegisterHotkey(new KeyGesture(Key.Insert), ToggleOverlay);

                // PageUp: Increase crosshair size
                _hotkeyService.RegisterHotkey(new KeyGesture(Key.PageUp), () =>
                {
                    CurrentCrosshairSettings.Size = Math.Min(100, CurrentCrosshairSettings.Size + 2);
                });

                // PageDown: Decrease crosshair size
                _hotkeyService.RegisterHotkey(new KeyGesture(Key.PageDown), () =>
                {
                    CurrentCrosshairSettings.Size = Math.Max(2, CurrentCrosshairSettings.Size - 2);
                });

                // End: Toggle RGB animation mode
                _hotkeyService.RegisterHotkey(new KeyGesture(Key.End), () =>
                {
                    CurrentCrosshairSettings.RgbAnimated = !CurrentCrosshairSettings.RgbAnimated;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hotkey registration warning: {ex.Message}");
            }
        }

        private void OnGameProfileDetected(object? sender, CrosshairSettings newSettings)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                CurrentCrosshairSettings = newSettings;
                CrosshairSettingsVm = new CrosshairSettingsViewModel(CurrentCrosshairSettings);
            });
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
        public void ToggleOverlay()
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
