
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CrosshairApp.Overlay
{
    public partial class OverlayWindow : Window
    {
        public OverlayWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
