
using System;

namespace CrosshairApp.Models
{
    public class Preset
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public CrosshairSettings Settings { get; set; }

        public Preset(string name, CrosshairSettings settings)
        {
            Name = name;
            Settings = settings;
        }
    }
}
