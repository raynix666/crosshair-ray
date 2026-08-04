
using Avalonia.Input;
using System;

namespace CrosshairApp.Services
{
    public interface IHotkeyService
    {
        void RegisterHotkey(KeyGesture hotkey, Action action);
        void UnregisterHotkey(KeyGesture hotkey);
        void UnregisterAllHotkeys();
    }
}
