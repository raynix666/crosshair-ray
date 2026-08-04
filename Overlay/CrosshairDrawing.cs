using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Platform;
using Avalonia.Visuals.Platform;
using Avalonia.Threading;
using System;
using System.ComponentModel;
using System.Linq;
using CrosshairApp.Models;
using CrosshairApp.Helpers;

namespace CrosshairApp.Overlay
{
    public class CrosshairDrawing : Control
    {
        public static readonly StyledProperty<CrosshairSettings> SettingsProperty = 
            AvaloniaProperty.Register<CrosshairDrawing, CrosshairSettings>(nameof(Settings));

        public CrosshairSettings Settings
        {
            get => GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        public CrosshairDrawing()
        {
            _colorAnimationTimer.Interval = TimeSpan.FromMilliseconds(50);
            _colorAnimationTimer.Tick += (sender, e) => UpdateRgbColor();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (Settings == null) return;

            // Get the center of the control
            var centerX = Bounds.Width / 2;
            var centerY = Bounds.Height / 2;

            if (centerX <= 0 || centerY <= 0) return;

            // Create a pen and brush for the crosshair
            var brush = new SolidColorBrush(Settings.Color) { Opacity = Settings.Opacity };
            var pen = new Pen(brush, Settings.Thickness) { LineCap = PenLineCap.Flat };

            // Apply outline if enabled
            if (Settings.OutlineEnabled)
            {
                var outlineBrush = new SolidColorBrush(Settings.OutlineColor) { Opacity = Settings.Opacity };
                var outlinePen = new Pen(outlineBrush, Settings.Thickness + Settings.OutlineThickness * 2) { LineCap = PenLineCap.Flat };
                // Draw outline first
                DrawCrosshair(context, outlinePen, centerX, centerY);
            }

            // Draw the main crosshair
            DrawCrosshair(context, pen, centerX, centerY);

            // Draw dot if enabled
            if (Settings.DotEnabled || Settings.Style == CrosshairStyle.Dot)
            {
                var dotRadius = Math.Max(2, Settings.Thickness);
                context.DrawEllipse(brush, null, new Point(centerX, centerY), dotRadius, dotRadius);
            }
        }

        private DispatcherTimer _colorAnimationTimer = new DispatcherTimer();
        private CrosshairApp.Helpers.HslColor _currentHslColor = new CrosshairApp.Helpers.HslColor(1, 1, 0.5);

        private void UpdateRgbColor()
        {
            if (Settings != null && Settings.RgbAnimated)
            {
                _currentHslColor = _currentHslColor.WithHue((_currentHslColor.H + 5) % 360);
                Settings.Color = _currentHslColor.ToRgb();
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == SettingsProperty)
            {
                if (change.OldValue is CrosshairSettings oldSettings)
                {
                    oldSettings.PropertyChanged -= OnSettingsPropertyChanged;
                }
                if (change.NewValue is CrosshairSettings newSettings)
                {
                    newSettings.PropertyChanged += OnSettingsPropertyChanged;
                    if (newSettings.RgbAnimated)
                    {
                        _colorAnimationTimer.Start();
                    }
                    else
                    {
                        _colorAnimationTimer.Stop();
                    }
                }
                Dispatcher.UIThread.Post(InvalidateVisual);
            }
        }

        private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CrosshairSettings.RgbAnimated))
            {
                if (Settings?.RgbAnimated == true)
                    _colorAnimationTimer.Start();
                else
                    _colorAnimationTimer.Stop();
            }
            Dispatcher.UIThread.Post(InvalidateVisual);
        }

        private void DrawCrosshair(DrawingContext context, Pen pen, double centerX, double centerY)
        {
            var halfSize = Settings.Size / 2;
            var gap = Settings.Gap;

            // Rotate the crosshair
            var rotationMatrix = Matrix.CreateTranslation(-centerX, -centerY) * Matrix.CreateRotation(Settings.Rotation * Math.PI / 180) * Matrix.CreateTranslation(centerX, centerY);
            using (context.PushTransform(rotationMatrix))
            {
                switch (Settings.Style)
                {
                    case CrosshairStyle.Cross:
                        // Horizontal line
                        context.DrawLine(pen, new Point(centerX - halfSize - gap, centerY), new Point(centerX - gap, centerY));
                        context.DrawLine(pen, new Point(centerX + gap, centerY), new Point(centerX + halfSize + gap, centerY));
                        // Vertical line
                        context.DrawLine(pen, new Point(centerX, centerY - halfSize - gap), new Point(centerX, centerY - gap));
                        context.DrawLine(pen, new Point(centerX, centerY + gap), new Point(centerX, centerY + halfSize + gap));
                        break;
                    case CrosshairStyle.Dot:
                        // Dot is handled separately in Render
                        break;
                    case CrosshairStyle.Circle:
                        context.DrawEllipse(null, pen, new Point(centerX, centerY), Math.Max(2, halfSize), Math.Max(2, halfSize));
                        break;
                }
            }
        }
    }
}
