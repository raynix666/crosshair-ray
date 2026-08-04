
using System;

namespace CrosshairApp.Models
{
    public class GameProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string GameName { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty; // e.g., "VALORANT-Win64-Shipping"
        public Guid SelectedPresetId { get; set; }

        // This property is for UI binding and will be set by the ViewModel
        public Preset? SelectedPreset { get; set; }

        public GameProfile(string gameName, string processName, Guid selectedPresetId)
        {
            GameName = gameName;
            ProcessName = processName;
            SelectedPresetId = selectedPresetId;
        }

        // Parameterless constructor for deserialization
        public GameProfile() { }
    }
}
