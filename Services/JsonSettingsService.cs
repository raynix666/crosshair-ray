using CrosshairApp.Models;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace CrosshairApp.Services
{
    public class JsonSettingsService : ISettingsService
    {
        private const string SettingsFileName = "settings.json";
        private const string StartupRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "CrosshairApp";

        private string GetSettingsFilePath()
        {
            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CrosshairApp");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            return Path.Combine(appDataPath, SettingsFileName);
        }

        public async Task<AppSettings> LoadSettingsAsync()
        {
            var filePath = GetSettingsFilePath();
            AppSettings settings = new AppSettings();
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            return settings;
        }

        public async Task SaveSettingsAsync(AppSettings settings)
        {
            var filePath = GetSettingsFilePath();
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);

            ApplyStartupRegistry(settings.LaunchAtStartup);
        }

        private void ApplyStartupRegistry(bool enable)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true);
                if (key != null)
                {
                    if (enable)
                    {
                        var exePath = Process.GetCurrentProcess().MainModule?.FileName;
                        if (!string.IsNullOrEmpty(exePath))
                        {
                            key.SetValue(AppName, $"\"{exePath}\"");
                        }
                    }
                    else
                    {
                        key.DeleteValue(AppName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Startup registry update error: {ex.Message}");
            }
        }
    }
}
