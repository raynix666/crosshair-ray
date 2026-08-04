using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrosshairApp.Services
{
    public class AvaloniaFileDialogService : IFileDialogService
    {
        private Window? _parentWindow;

        public void SetParentWindow(Window window)
        {
            _parentWindow = window;
        }

        public async Task<string?> OpenFileAsync(string title, string filter)
        {
            if (_parentWindow == null) return null;

            var topLevel = TopLevel.GetTopLevel(_parentWindow);
            if (topLevel?.StorageProvider == null) return null;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("JSON files") { Patterns = new[] { "*.json" } },
                    FilePickerFileTypes.All
                }
            });

            return files.Count > 0 ? files[0].Path.LocalPath : null;
        }

        public async Task<string?> SaveFileAsync(string title, string defaultFileName, string filter)
        {
            if (_parentWindow == null) return null;

            var topLevel = TopLevel.GetTopLevel(_parentWindow);
            if (topLevel?.StorageProvider == null) return null;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = title,
                SuggestedFileName = defaultFileName,
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("JSON files") { Patterns = new[] { "*.json" } },
                    FilePickerFileTypes.All
                }
            });

            return file?.Path.LocalPath;
        }
    }
}
