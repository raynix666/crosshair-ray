using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CrosshairApp.Models;
using CrosshairApp.Overlay;
using CrosshairApp.ViewModels;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CrosshairApp.Services
{
    public class WindowsOverlayService : IOverlayService
    {
        private OverlayWindow? _overlayWindow;
        private CrosshairSettings? _currentSettings;

        public bool IsOverlayVisible => _overlayWindow != null && _overlayWindow.IsVisible;

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
            });
        }

        public void HideOverlay()
        {
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
            if (_overlayWindow != null)
            {
                var primaryScreen = _overlayWindow.Screens.Primary;
                if (primaryScreen != null)
                {
                    var screen = primaryScreen.WorkingArea;
                    var x = screen.X + (screen.Width - 300) / 2;
                    var y = screen.Y + (screen.Height - 300) / 2;

                    SetOverlayPosition(x, y, 300, 300);
                }
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
                    // WS_EX_LAYERED (0x80000) | WS_EX_TRANSPARENT (0x20) | WS_EX_TOOLWINDOW (0x80)
                    extendedStyle |= GWL_EXSTYLE_LAYERED | GWL_EXSTYLE_TRANSPARENT | 0x80;
                    SetWindowLong(platformHandle, GWL_EXSTYLE, extendedStyle);
                }
            }
        }

        private const int GWL_EXSTYLE = -20;
        private const int GWL_EXSTYLE_LAYERED = 0x80000;
        private const int GWL_EXSTYLE_TRANSPARENT = 0x20;
        private const int LWA_ALPHA = 0x2;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
    }
}
