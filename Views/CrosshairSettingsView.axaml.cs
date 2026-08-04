using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CrosshairApp.Views;

public partial class CrosshairSettingsView : UserControl
{
    public CrosshairSettingsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
