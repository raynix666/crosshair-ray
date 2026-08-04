using Avalonia;
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
            var overlayService = new WindowsOverlayService();
            var settingsService = new JsonSettingsService();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(overlayService, settingsService),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}