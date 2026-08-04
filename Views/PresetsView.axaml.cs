using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CrosshairApp.Views;

public partial class PresetsView : UserControl
{
    public PresetsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
