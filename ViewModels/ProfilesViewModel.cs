
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrosshairApp.Models;
using CrosshairApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CrosshairApp.ViewModels
{
    public partial class ProfilesViewModel : ViewModelBase
    {
        private readonly IProfileService _profileService;

        [ObservableProperty]
        private ObservableCollection<GameProfile> _gameProfiles;

        [ObservableProperty]
        private ObservableCollection<Preset> _allPresets;

        [ObservableProperty]
        private GameProfile? _selectedGameProfile;

        public ProfilesViewModel(IProfileService profileService, ObservableCollection<GameProfile> gameProfiles, ObservableCollection<Preset> allPresets)
        {
            _profileService = profileService;
            _gameProfiles = gameProfiles;
            _allPresets = allPresets;
        }

        partial void OnSelectedGameProfileChanged(GameProfile? value)
        {
            if (value != null)
            {
                value.SelectedPreset = AllPresets.FirstOrDefault(p => p.Id == value.SelectedPresetId);
            }
        }

        [RelayCommand]
        private async Task AddGameProfile()
        {
            var newProfile = new GameProfile("New Game", "", Guid.Empty);
            GameProfiles.Add(newProfile);
            SelectedGameProfile = newProfile;
            await _profileService.AddProfileAsync(newProfile);
        }

        [RelayCommand]
        private async Task SaveGameProfile()
        {
            if (SelectedGameProfile != null)
            {
                await _profileService.UpdateProfileAsync(SelectedGameProfile);
            }
        }

        [RelayCommand]
        private async Task DeleteGameProfile()
        {
            if (SelectedGameProfile != null)
            {
                await _profileService.DeleteProfileAsync(SelectedGameProfile);
                GameProfiles.Remove(SelectedGameProfile);
                SelectedGameProfile = GameProfiles.FirstOrDefault();
            }
        }
    }
}
