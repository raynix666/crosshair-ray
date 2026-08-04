
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrosshairApp.Models;
using CrosshairApp.Services;
using System.Threading.Tasks;

namespace CrosshairApp.ViewModels
{
    public partial class HotkeysViewModel : ViewModelBase
    {
        private readonly IHotkeyService _hotkeyService;
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private HotkeySettings _hotkeySettings;

        public HotkeysViewModel(IHotkeyService hotkeyService, ISettingsService settingsService, HotkeySettings hotkeySettings)
        {
            _hotkeyService = hotkeyService;
            _settingsService = settingsService;
            _hotkeySettings = hotkeySettings;
        }

        partial void OnHotkeySettingsChanged(HotkeySettings value)
        {
            // When hotkey settings change, re-register hotkeys
            _hotkeyService.UnregisterAllHotkeys();
            // Re-register hotkeys based on the updated settings
            // This part will need to be handled by MainViewModel or a dedicated service
            // Save settings via MainViewModel or settings service when needed
        }

        // Commands for changing individual hotkeys will be implemented here
        // This will likely involve a dialog to capture the new key combination
    }
}
