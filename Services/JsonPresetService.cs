
using CrosshairApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace CrosshairApp.Services
{
    public class JsonPresetService : IPresetService
    {
        private const string PresetsFileName = "presets.json";

        private string GetPresetsFilePath()
        {
            // Store presets in application data directory
            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CrosshairApp");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            return Path.Combine(appDataPath, PresetsFileName);
        }

        public async Task<List<Preset>> LoadPresetsAsync()
        {
            var filePath = GetPresetsFilePath();
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<List<Preset>>(json) ?? new List<Preset>();
            }
            return new List<Preset>();
        }

        public async Task SavePresetsAsync(List<Preset> presets)
        {
            var filePath = GetPresetsFilePath();
            var json = JsonSerializer.Serialize(presets, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task AddPresetAsync(Preset preset)
        {
            var presets = await LoadPresetsAsync();
            presets.Add(preset);
            await SavePresetsAsync(presets);
        }

        public async Task UpdatePresetAsync(Preset preset)
        {
            var presets = await LoadPresetsAsync();
            var existingPreset = presets.Find(p => p.Id == preset.Id);
            if (existingPreset != null)
            {
                existingPreset.Name = preset.Name;
                existingPreset.Settings = preset.Settings;
            }
            await SavePresetsAsync(presets);
        }

        public async Task DeletePresetAsync(Preset preset)
        {
            var presets = await LoadPresetsAsync();
            presets.RemoveAll(p => p.Id == preset.Id);
            await SavePresetsAsync(presets);
        }

        public async Task ExportPresetToJsonAsync(Preset preset, string filePath)
        {
            var json = JsonSerializer.Serialize(preset, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<Preset?> ImportPresetFromJsonAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<Preset>(json);
            }
            return null;
        }
    }
}
