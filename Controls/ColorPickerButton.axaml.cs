using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace CrosshairApp.Controls
{
    public partial class ColorPickerButton : UserControl
    {
        public static readonly StyledProperty<Color> SelectedColorProperty =
            AvaloniaProperty.Register<ColorPickerButton, Color>(
                nameof(SelectedColor), 
                Colors.Red, 
                defaultBindingMode: BindingMode.TwoWay);

        public Color SelectedColor
        {
            get => GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public ColorPickerButton()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnPresetColorClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string colorStr)
            {
                if (Color.TryParse(colorStr, out var parsedColor))
                {
                    SetCurrentValue(SelectedColorProperty, parsedColor);
                }
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == SelectedColorProperty)
            {
                var swatch = this.FindControl<Border>("PART_ColorSwatch");
                if (swatch != null && change.NewValue is Color newColor)
                {
                    swatch.Background = new SolidColorBrush(newColor);
                }
            }
        }
    }
}
