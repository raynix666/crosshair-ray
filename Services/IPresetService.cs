
using CrosshairApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrosshairApp.Services
{
    public interface IPresetService
    {
        Task<List<Preset>> LoadPresetsAsync();
        Task SavePresetsAsync(List<Preset> presets);
        Task AddPresetAsync(Preset preset);
        Task UpdatePresetAsync(Preset preset);
        Task DeletePresetAsync(Preset preset);
        Task ExportPresetToJsonAsync(Preset preset, string filePath);
        Task<Preset?> ImportPresetFromJsonAsync(string filePath);
    }
}
