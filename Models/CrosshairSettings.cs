
using Avalonia.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CrosshairApp.Helpers;

namespace CrosshairApp.Models
{
    public class CrosshairSettings : INotifyPropertyChanged
    {
        private Color _color = Colors.Red;
        private bool _rgbAnimated = false;
        private double _size = 20;
        private double _thickness = 2;
        private double _opacity = 1.0;
        private double _rotation = 0;
        private double _gap = 0;
        private bool _outlineEnabled = false;
        private double _outlineThickness = 1;
        private Color _outlineColor = Colors.Black;
        private bool _shadowEnabled = false;
        private double _shadowBlur = 0;
        private bool _dotEnabled = false;
        private CrosshairStyle _style = CrosshairStyle.Cross;

        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public bool RgbAnimated
        {
            get => _rgbAnimated;
            set => SetProperty(ref _rgbAnimated, value);
        }

        public double Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        public double Thickness
        {
            get => _thickness;
            set => SetProperty(ref _thickness, value);
        }

        public double Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, value);
        }

        public double Rotation
        {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }

        public double Gap
        {
            get => _gap;
            set => SetProperty(ref _gap, value);
        }

        public bool OutlineEnabled
        {
            get => _outlineEnabled;
            set => SetProperty(ref _outlineEnabled, value);
        }

        public double OutlineThickness
        {
            get => _outlineThickness;
            set => SetProperty(ref _outlineThickness, value);
        }

        public Color OutlineColor
        {
            get => _outlineColor;
            set => SetProperty(ref _outlineColor, value);
        }

        public bool ShadowEnabled
        {
            get => _shadowEnabled;
            set => SetProperty(ref _shadowEnabled, value);
        }

        public double ShadowBlur
        {
            get => _shadowBlur;
            set => SetProperty(ref _shadowBlur, value);
        }

        public bool DotEnabled
        {
            get => _dotEnabled;
            set => SetProperty(ref _dotEnabled, value);
        }

        public CrosshairStyle Style
        {
            get => _style;
            set => SetProperty(ref _style, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public enum CrosshairStyle
    {
        Cross,
        Dot,
        Circle
    }
}
