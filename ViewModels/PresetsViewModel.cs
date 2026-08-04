
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrosshairApp.Models;
using CrosshairApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CrosshairApp.ViewModels
{
    public partial class PresetsViewModel : ViewModelBase
    {
        private readonly IPresetService _presetService;
        private readonly IFileDialogService _fileDialogService;

        [ObservableProperty]
        private ObservableCollection<Preset> _presets;

        [ObservableProperty]
        private Preset? _selectedPreset;

        [ObservableProperty]
        private CrosshairSettingsViewModel? _selectedPresetViewModel;

        partial void OnSelectedPresetChanged(Preset? value)
        {
            SelectedPresetViewModel = value != null ? new CrosshairSettingsViewModel(value.Settings) : null;
        }

        public PresetsViewModel(IPresetService presetService, ObservableCollection<Preset> presets, IFileDialogService fileDialogService)
        {
            _presetService = presetService;
            _presets = presets;
            _fileDialogService = fileDialogService;
        }

        [RelayCommand]
        private async Task AddPreset()
        {
            var newPreset = new Preset("New Preset", new CrosshairSettings());
            Presets.Add(newPreset);
            SelectedPreset = newPreset;
            await _presetService.AddPresetAsync(newPreset);
        }

        [RelayCommand]
        private async Task SavePreset()
        {
            if (SelectedPreset != null)
            {
                await _presetService.UpdatePresetAsync(SelectedPreset);
            }
        }

        [RelayCommand]
        private async Task DeletePreset()
        {
            if (SelectedPreset != null)
            {
                await _presetService.DeletePresetAsync(SelectedPreset);
                Presets.Remove(SelectedPreset);
                SelectedPreset = Presets.FirstOrDefault();
            }
        }

                [RelayCommand]
        private async Task ImportPreset()
        {
            var filePath = await _fileDialogService.OpenFileAsync("Import Preset", "JSON files|*.json");
            if (!string.IsNullOrEmpty(filePath))
            {
                var importedPreset = await _presetService.ImportPresetFromJsonAsync(filePath);
                if (importedPreset != null)
                {
                    Presets.Add(importedPreset);
                    SelectedPreset = importedPreset;
                    await _presetService.AddPresetAsync(importedPreset);
                }
            }
        }

        [RelayCommand]
        private async Task ExportPreset()
        {
            if (SelectedPreset == null) return;

            var filePath = await _fileDialogService.SaveFileAsync("Export Preset", SelectedPreset.Name + ".json", "JSON files|*.json");
            if (!string.IsNullOrEmpty(filePath))
            {
                await _presetService.ExportPresetToJsonAsync(SelectedPreset, filePath);
            }
        }
    }
}
