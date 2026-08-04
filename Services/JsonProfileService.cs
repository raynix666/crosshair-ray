
using CrosshairApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CrosshairApp.Services
{
    public class JsonProfileService : IProfileService
    {
        private const string ProfilesFileName = "profiles.json";

        private string GetProfilesFilePath()
        {
            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CrosshairApp");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            return Path.Combine(appDataPath, ProfilesFileName);
        }

        public async Task<List<GameProfile>> LoadProfilesAsync()
        {
            var filePath = GetProfilesFilePath();
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<List<GameProfile>>(json) ?? new List<GameProfile>();
            }
            return new List<GameProfile>();
        }

        public async Task SaveProfilesAsync(List<GameProfile> profiles)
        {
            var filePath = GetProfilesFilePath();
            var json = JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task AddProfileAsync(GameProfile profile)
        {
            var profiles = await LoadProfilesAsync();
            profiles.Add(profile);
            await SaveProfilesAsync(profiles);
        }

        public async Task UpdateProfileAsync(GameProfile profile)
        {
            var profiles = await LoadProfilesAsync();
            var existingProfile = profiles.Find(p => p.Id == profile.Id);
            if (existingProfile != null)
            {
                existingProfile.GameName = profile.GameName;
                existingProfile.ProcessName = profile.ProcessName;
                existingProfile.SelectedPresetId = profile.SelectedPresetId;
            }
            await SaveProfilesAsync(profiles);
        }

        public async Task DeleteProfileAsync(GameProfile profile)
        {
            var profiles = await LoadProfilesAsync();
            profiles.RemoveAll(p => p.Id == profile.Id);
            await SaveProfilesAsync(profiles);
        }

        public async Task<GameProfile?> GetActiveProfileAsync()
        {
            var profiles = await LoadProfilesAsync();
            var runningProcesses = Process.GetProcesses().Select(p => p.ProcessName).ToList();

            foreach (var profile in profiles)
            {
                if (runningProcesses.Contains(profile.ProcessName))
                {
                    return profile;
                }
            }
            return null;
        }
    }
}
