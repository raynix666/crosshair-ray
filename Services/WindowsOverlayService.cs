using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CrosshairApp.Models;
using CrosshairApp.Overlay;
using CrosshairApp.ViewModels;
using System;
using System.Runtime.InteropServices;

namespace CrosshairApp.Services
{
    public class WindowsOverlayService : IOverlayService
    {
        private OverlayWindow? _overlayWindow;
        private CrosshairSettings? _currentSettings;
        private DispatcherTimer? _topmostEnforceTimer;

        // Track last known screen size so we reposition when resolution changes
        private int _lastScreenWidth = 0;
        private int _lastScreenHeight = 0;

        // Overlay size is fixed at 300x300 but centered on actual screen resolution
        private const int OverlaySize = 300;

        public bool IsOverlayVisible => _overlayWindow != null && _overlayWindow.IsVisible;

        public WindowsOverlayService()
        {
            _topmostEnforceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            _topmostEnforceTimer.Tick += (s, e) => EnsureTopmostAndReposition();
        }

        public void ShowOverlay(CrosshairSettings settings)
        {
            _currentSettings = settings;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (_overlayWindow == null)
                {
                    _overlayWindow = new OverlayWindow
                    {
                        DataContext = new OverlayWindowViewModel(settings)
                    };
                    _overlayWindow.Show();
                    ApplyWin32Styles(_overlayWindow);
                    CenterOverlayOnPrimaryScreen();
                }
                else
                {
                    if (_overlayWindow.DataContext is OverlayWindowViewModel vm)
                    {
                        vm.CrosshairSettings = settings;
                    }
                    _overlayWindow.Show();
                    ApplyWin32Styles(_overlayWindow);
                    CenterOverlayOnPrimaryScreen();
                }

                _topmostEnforceTimer?.Start();
            });
        }

        public void HideOverlay()
        {
            _topmostEnforceTimer?.Stop();
            if (_overlayWindow != null)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _overlayWindow.Hide();
                });
            }
        }

        public void CloseOverlay()
        {
            _topmostEnforceTimer?.Stop();
            if (_overlayWindow != null)
            {
                var window = _overlayWindow;
                _overlayWindow = null;
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    window.Close();
                });
            }
        }

        public void UpdateOverlaySettings(CrosshairSettings settings)
        {
            _currentSettings = settings;
            if (_overlayWindow != null && _overlayWindow.DataContext is OverlayWindowViewModel vm)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    vm.CrosshairSettings = settings;
                });
            }
        }

        public void SetOverlayPosition(double x, double y, double width, double height)
        {
            if (_overlayWindow != null)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _overlayWindow.Position = new PixelPoint((int)x, (int)y);
                    _overlayWindow.Width = width;
                    _overlayWindow.Height = height;
                });
            }
        }

        private void CenterOverlayOnPrimaryScreen()
        {
            if (_overlayWindow == null) return;

            // Get actual screen pixel resolution using Win32 (works in exclusive fullscreen)
            int screenW = GetSystemMetrics(SM_CXSCREEN);
            int screenH = GetSystemMetrics(SM_CYSCREEN);

            // Fallback to Avalonia screen bounds if Win32 returns 0
            if (screenW <= 0 || screenH <= 0)
            {
                var primary = _overlayWindow.Screens.Primary;
                if (primary == null) return;
                screenW = primary.Bounds.Width;
                screenH = primary.Bounds.Height;
            }

            _lastScreenWidth = screenW;
            _lastScreenHeight = screenH;

            int x = (screenW - OverlaySize) / 2;
            int y = (screenH - OverlaySize) / 2;

            _overlayWindow.Position = new PixelPoint(x, y);
            _overlayWindow.Width = OverlaySize;
            _overlayWindow.Height = OverlaySize;
        }

        private void EnsureTopmostAndReposition()
        {
            if (_overlayWindow == null || !_overlayWindow.IsVisible) return;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            // Check if screen resolution has changed
            int screenW = GetSystemMetrics(SM_CXSCREEN);
            int screenH = GetSystemMetrics(SM_CYSCREEN);

            if (screenW > 0 && screenH > 0 &&
                (screenW != _lastScreenWidth || screenH != _lastScreenHeight))
            {
                // Resolution changed — reposition to new center
                _lastScreenWidth = screenW;
                _lastScreenHeight = screenH;

                int x = (screenW - OverlaySize) / 2;
                int y = (screenH - OverlaySize) / 2;

                var platformHandle = _overlayWindow.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
                if (platformHandle != IntPtr.Zero)
                {
                    // Move AND keep topmost in one call
                    SetWindowPos(platformHandle, HWND_TOPMOST,
                        x, y, OverlaySize, OverlaySize,
                        SWP_NOACTIVATE | SWP_SHOWWINDOW);
                }
                return;
            }

            // No resolution change — just keep on top
            var handle = _overlayWindow.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
            if (handle != IntPtr.Zero)
            {
                SetWindowPos(handle, HWND_TOPMOST,
                    0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
            }
        }

        private void ApplyWin32Styles(Window window)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var platformHandle = window.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
                if (platformHandle != IntPtr.Zero)
                {
                    int extendedStyle = GetWindowLong(platformHandle, GWL_EXSTYLE);
                    extendedStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE | WS_EX_TOPMOST;
                    SetWindowLong(platformHandle, GWL_EXSTYLE, extendedStyle);

                    SetWindowPos(platformHandle, HWND_TOPMOST,
                        0, 0, 0, 0,
                        SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
                }
            }
        }

        private const int GWL_EXSTYLE    = -20;
        private const int WS_EX_TOPMOST  = 0x00000008;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_LAYERED  = 0x00080000;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOSIZE     = 0x0001;
        private const uint SWP_NOMOVE     = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);
    }
}
