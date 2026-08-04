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
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var overlayService = new WindowsOverlayService();
            var settingsService = new JsonSettingsService();

            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel(overlayService, settingsService),
            };

            mainWindow.Closed += (sender, e) =>
            {
                overlayService.CloseOverlay();
            };

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}