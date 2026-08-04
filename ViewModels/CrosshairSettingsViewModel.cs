
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrosshairApp.Models;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

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

        [RelayCommand]
        private async Task BrowseCustomImageAsync()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            {
                var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
                if (topLevel?.StorageProvider != null)
                {
                    var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = "Select Crosshair Image",
                        AllowMultiple = false,
                        FileTypeFilter = new[]
                        {
                            new FilePickerFileType("Image files") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.webp", "*.svg" } },
                            FilePickerFileTypes.All
                        }
                    });

                    if (files.Count > 0)
                    {
                        Settings.CustomImagePath = files[0].Path.LocalPath;
                        Settings.Style = CrosshairStyle.CustomImage;
                    }
                }
            }
        }
    }
}
