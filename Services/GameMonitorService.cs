using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia.Threading;
using CrosshairApp.Models;

namespace CrosshairApp.Services
{
    public class GameMonitorService
    {
        private readonly DispatcherTimer _timer;
        private List<GameProfile> _profiles = new List<GameProfile>();
        private List<Preset> _presets = new List<Preset>();
        private string _lastDetectedProcess = string.Empty;

        public event EventHandler<CrosshairSettings>? ProfileDetected;

        public GameMonitorService()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _timer.Tick += OnCheckRunningProcesses;
        }

        public void Start(IEnumerable<GameProfile> profiles, IEnumerable<Preset> presets)
        {
            UpdateData(profiles, presets);
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void UpdateData(IEnumerable<GameProfile> profiles, IEnumerable<Preset> presets)
        {
            _profiles = profiles.ToList();
            _presets = presets.ToList();
        }

        private void OnCheckRunningProcesses(object? sender, EventArgs e)
        {
            if (!_profiles.Any()) return;

            try
            {
                var runningProcesses = Process.GetProcesses()
                    .Select(p => p.ProcessName)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                foreach (var profile in _profiles)
                {
                    if (string.IsNullOrWhiteSpace(profile.ProcessName)) continue;

                    var cleanProcessName = profile.ProcessName.Replace(".exe", "", StringComparison.OrdinalIgnoreCase).Trim();

                    if (runningProcesses.Contains(cleanProcessName))
                    {
                        if (_lastDetectedProcess != cleanProcessName)
                        {
                            _lastDetectedProcess = cleanProcessName;
                            var preset = _presets.FirstOrDefault(p => p.Id == profile.SelectedPresetId);
                            if (preset != null && preset.Settings != null)
                            {
                                ProfileDetected?.Invoke(this, preset.Settings);
                            }
                        }
                        return; // Found match
                    }
                }

                _lastDetectedProcess = string.Empty;
            }
            catch
            {
                // Process inspection exception safety
            }
        }
    }
}
