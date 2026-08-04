
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CrosshairApp.Services
{
    public class WindowsHotkeyService : IHotkeyService
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int WM_HOTKEY = 0x0312;
        private Dictionary<int, Action> _hotkeyActions = new Dictionary<int, Action>();
        private Dictionary<KeyGesture, int> _registeredHotkeys = new Dictionary<KeyGesture, int>();
        private int _currentHotkeyId = 0;
        private IntPtr _hWnd = IntPtr.Zero; // This would typically be a hidden window handle

        public WindowsHotkeyService()
        {
            // In a real application, you would create a hidden window to receive WM_HOTKEY messages.
            // For this sandbox environment, we'll simulate the registration.
            // This implementation will not actually register global hotkeys due to sandbox limitations.
        }

        public void RegisterHotkey(KeyGesture hotkey, Action action)
        {
            if (hotkey == null) return;

            _currentHotkeyId++;
            int id = _currentHotkeyId;

            uint fsModifiers = ConvertModifiers(hotkey.KeyModifiers);
            uint vk = ConvertKey(hotkey.Key);

            // In a real Windows application, you would call RegisterHotKey here.
            // For now, we'll just store the action.
            _hotkeyActions[id] = action;
            _registeredHotkeys[hotkey] = id;

            Console.WriteLine($"Simulating hotkey registration: {hotkey.Key} with modifiers {hotkey.KeyModifiers}");
        }

        public void UnregisterHotkey(KeyGesture hotkey)
        {
            if (_registeredHotkeys.TryGetValue(hotkey, out int id))
            {
                // In a real application, you would call UnregisterHotKey here.
                _hotkeyActions.Remove(id);
                _registeredHotkeys.Remove(hotkey);
                Console.WriteLine($"Simulating hotkey unregistration: {hotkey.Key} with modifiers {hotkey.KeyModifiers}");
            }
        }

        public void UnregisterAllHotkeys()
        {
            foreach (var entry in _registeredHotkeys)
            {
                // In a real application, you would call UnregisterHotKey here.
                Console.WriteLine($"Simulating unregistration of hotkey: {entry.Key.Key} with modifiers {entry.Key.KeyModifiers}");
            }
            _hotkeyActions.Clear();
            _registeredHotkeys.Clear();
            _currentHotkeyId = 0;
        }

        // This method would be called by the message loop when a WM_HOTKEY message is received
        public void OnHotkey(int id)
        {
            if (_hotkeyActions.TryGetValue(id, out var action))
            {
                action?.Invoke();
            }
        }

        // Helper to convert Avalonia KeyModifiers to Win32 modifiers
        private uint ConvertModifiers(KeyModifiers modifiers)
        {
            uint fsModifiers = 0;
            if (modifiers.HasFlag(KeyModifiers.Alt)) fsModifiers |= 0x0001; // MOD_ALT
            if (modifiers.HasFlag(KeyModifiers.Control)) fsModifiers |= 0x0002; // MOD_CONTROL
            if (modifiers.HasFlag(KeyModifiers.Shift)) fsModifiers |= 0x0004; // MOD_SHIFT
            if (modifiers.HasFlag(KeyModifiers.Meta)) fsModifiers |= 0x0008; // MOD_WIN
            return fsModifiers;
        }

        // Helper to convert Avalonia Key to Win32 virtual key code
        private uint ConvertKey(Key key)
        {
            // This is a simplified mapping. A full implementation would be more extensive.
            // For a robust solution, consider a comprehensive lookup table or a library.
            return key switch
            {
                Key.F8 => 0x77, // VK_F8
                Key.PageUp => 0x21, // VK_PRIOR
                Key.PageDown => 0x22, // VK_NEXT
                Key.Home => 0x24, // VK_HOME
                Key.End => 0x23, // VK_END
                Key.Insert => 0x2D, // VK_INSERT
                _ => 0x00, // Default to 0 or throw an exception for unsupported keys
            };
        }
    }
}
