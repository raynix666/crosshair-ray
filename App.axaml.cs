using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CrosshairApp.ViewModels;
using CrosshairApp.Services;
using CrosshairApp.Views;

namespace CrosshairApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var overlayService = new WindowsOverlayService();
            var settingsService = new JsonSettingsService();
            var hotkeyService = new WindowsHotkeyService();
            var gameMonitorService = new GameMonitorService();
            var profileService = new JsonProfileService();
            var presetService = new JsonPresetService();

            var mainViewModel = new MainViewModel(
                overlayService,
                settingsService,
                hotkeyService,
                gameMonitorService,
                profileService,
                presetService);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };

            desktop.MainWindow = mainWindow;

            // Setup System Tray Icon
            SetupSystemTray(desktop, mainWindow, mainViewModel, overlayService);

            mainWindow.Closing += (sender, e) =>
            {
                if (mainViewModel.AppSettings.MinimizeToTrayOnClose)
                {
                    e.Cancel = true;
                    mainWindow.Hide();
                }
                else
                {
                    overlayService.CloseOverlay();
                    desktop.Shutdown();
                }
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void SetupSystemTray(
        IClassicDesktopStyleApplicationLifetime desktop,
        MainWindow mainWindow,
        MainViewModel mainViewModel,
        IOverlayService overlayService)
    {
        try
        {
            var trayIcon = new TrayIcon
            {
                ToolTipText = "CrosshairApp",
                IsVisible = true
            };

            var toggleItem = new NativeMenuItem("Toggle Crosshair");
            toggleItem.Click += (s, e) => mainViewModel.ToggleOverlayCommand.Execute(null);

            var showSettingsItem = new NativeMenuItem("Open Settings");
            showSettingsItem.Click += (s, e) =>
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            };

            var exitItem = new NativeMenuItem("Exit App");
            exitItem.Click += (s, e) =>
            {
                overlayService.CloseOverlay();
                desktop.Shutdown();
            };

            var nativeMenu = new NativeMenu();
            nativeMenu.Add(toggleItem);
            nativeMenu.Add(showSettingsItem);
            nativeMenu.Add(new NativeMenuItemSeparator());
            nativeMenu.Add(exitItem);

            trayIcon.Menu = nativeMenu;
            trayIcon.Clicked += (s, e) =>
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            };

            var icons = new TrayIcons { trayIcon };
            TrayIcon.SetIcons(this, icons);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"System tray initialization error: {ex.Message}");
        }
    }
}