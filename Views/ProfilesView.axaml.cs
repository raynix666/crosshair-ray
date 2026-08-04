using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CrosshairApp.Views;

public partial class ProfilesView : UserControl
{
    public ProfilesView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
