using Avalonia.Input;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CrosshairApp.Services
{
    public class WindowsHotkeyService : IHotkeyService
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            public WndProcDelegate lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string? lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_HOTKEY = 0x0312;
        private const string WindowClassName = "CrosshairApp_Hotkey_MsgWindow";
        private static readonly IntPtr HWND_MESSAGE = new IntPtr(-3);

        private Dictionary<int, Action> _hotkeyActions = new Dictionary<int, Action>();
        private Dictionary<KeyGesture, int> _registeredHotkeys = new Dictionary<KeyGesture, int>();
        private int _currentHotkeyId = 0;
        private IntPtr _hWnd = IntPtr.Zero;
        private WndProcDelegate? _wndProcDelegate;

        public WindowsHotkeyService()
        {
            EnsureMessageWindowCreated();
        }

        private void EnsureMessageWindowCreated()
        {
            if (_hWnd != IntPtr.Zero || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            try
            {
                _wndProcDelegate = WndProc;
                var wndClass = new WNDCLASSEX
                {
                    cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
                    lpfnWndProc = _wndProcDelegate,
                    hInstance = GetModuleHandle(null),
                    lpszClassName = WindowClassName
                };

                RegisterClassEx(ref wndClass);

                _hWnd = CreateWindowEx(
                    0,
                    WindowClassName,
                    "HotkeyListener",
                    0,
                    0, 0, 0, 0,
                    HWND_MESSAGE,
                    IntPtr.Zero,
                    GetModuleHandle(null),
                    IntPtr.Zero);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing global hotkey window: {ex.Message}");
            }
        }

        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                OnHotkey(id);
                return IntPtr.Zero;
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        public void RegisterHotkey(KeyGesture hotkey, Action action)
        {
            if (hotkey == null) return;

            EnsureMessageWindowCreated();

            _currentHotkeyId++;
            int id = _currentHotkeyId;

            uint fsModifiers = ConvertModifiers(hotkey.KeyModifiers) | 0x4000; // MOD_NOREPEAT
            uint vk = ConvertKey(hotkey.Key);

            if (vk != 0 && _hWnd != IntPtr.Zero && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                RegisterHotKey(_hWnd, id, fsModifiers, vk);
            }

            _hotkeyActions[id] = action;
            _registeredHotkeys[hotkey] = id;
        }

        public void UnregisterHotkey(KeyGesture hotkey)
        {
            if (_registeredHotkeys.TryGetValue(hotkey, out int id))
            {
                if (_hWnd != IntPtr.Zero && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    UnregisterHotKey(_hWnd, id);
                }
                _hotkeyActions.Remove(id);
                _registeredHotkeys.Remove(hotkey);
            }
        }

        public void UnregisterAllHotkeys()
        {
            if (_hWnd != IntPtr.Zero && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                foreach (var entry in _registeredHotkeys)
                {
                    UnregisterHotKey(_hWnd, entry.Value);
                }
            }
            _hotkeyActions.Clear();
            _registeredHotkeys.Clear();
            _currentHotkeyId = 0;
        }

        public void OnHotkey(int id)
        {
            if (_hotkeyActions.TryGetValue(id, out var action))
            {
                Dispatcher.UIThread.InvokeAsync(() => action?.Invoke());
            }
        }

        private uint ConvertModifiers(KeyModifiers modifiers)
        {
            uint fsModifiers = 0;
            if (modifiers.HasFlag(KeyModifiers.Alt)) fsModifiers |= 0x0001; // MOD_ALT
            if (modifiers.HasFlag(KeyModifiers.Control)) fsModifiers |= 0x0002; // MOD_CONTROL
            if (modifiers.HasFlag(KeyModifiers.Shift)) fsModifiers |= 0x0004; // MOD_SHIFT
            if (modifiers.HasFlag(KeyModifiers.Meta)) fsModifiers |= 0x0008; // MOD_WIN
            return fsModifiers;
        }

        private uint ConvertKey(Key key)
        {
            if (key >= Key.A && key <= Key.Z)
                return (uint)(0x41 + (key - Key.A));
            if (key >= Key.D0 && key <= Key.D9)
                return (uint)(0x30 + (key - Key.D0));
            if (key >= Key.F1 && key <= Key.F24)
                return (uint)(0x70 + (key - Key.F1));

            return key switch
            {
                Key.PageUp => 0x21,
                Key.PageDown => 0x22,
                Key.End => 0x23,
                Key.Home => 0x24,
                Key.Left => 0x25,
                Key.Up => 0x26,
                Key.Right => 0x27,
                Key.Down => 0x28,
                Key.Insert => 0x2D,
                Key.Delete => 0x2E,
                Key.Space => 0x20,
                Key.Return => 0x0D,
                Key.Escape => 0x1B,
                Key.Tab => 0x09,
                _ => 0x00,
            };
        }
    }
}
