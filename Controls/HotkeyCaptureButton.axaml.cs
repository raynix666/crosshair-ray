
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Linq;

using Avalonia.Markup.Xaml;

namespace CrosshairApp.Controls
{
    public partial class HotkeyCaptureButton : UserControl
    {
        public HotkeyCaptureButton()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public static readonly StyledProperty<KeyGesture?> HotkeyProperty =
            AvaloniaProperty.Register<HotkeyCaptureButton, KeyGesture?>(nameof(Hotkey));

        public KeyGesture? Hotkey
        {
            get => GetValue(HotkeyProperty);
            set => SetValue(HotkeyProperty, value);
        }

        public static readonly StyledProperty<string> HotkeyTextProperty =
            AvaloniaProperty.Register<HotkeyCaptureButton, string>(nameof(HotkeyText), "Click to set");

        public string HotkeyText
        {
            get => GetValue(HotkeyTextProperty);
            set => SetValue(HotkeyTextProperty, value);
        }

        private Button? _button;
        private bool _isCapturing = false;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _button = e.NameScope.Find<Button>("PART_Button");
            if (_button != null)
            {
                _button.Click += OnButtonClick;
            }
            UpdateHotkeyText();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == HotkeyProperty)
            {
                UpdateHotkeyText();
            }
        }

        private void OnButtonClick(object? sender, RoutedEventArgs e)
        {
            if (!_isCapturing)
            {
                StartCapture();
            }
            else
            {
                StopCapture();
            }
        }

        private void StartCapture()
        {
            _isCapturing = true;
            HotkeyText = "Press a key...";
            Focus(); // Ensure the control has focus to capture key events
            KeyDown += OnKeyDown;
            LostFocus += OnLostFocus;
        }

        private void StopCapture()
        {
            _isCapturing = false;
            KeyDown -= OnKeyDown;
            LostFocus -= OnLostFocus;
            UpdateHotkeyText();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            e.Handled = true; // Prevent the key event from propagating further

            Key key = e.Key;
            KeyModifiers modifiers = e.KeyModifiers;

            // Ignore modifier keys alone
            if (key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            // Create KeyGesture
            Hotkey = new KeyGesture(key, modifiers);
            StopCapture();
        }

        private void OnLostFocus(object? sender, RoutedEventArgs e)
        {
            // If focus is lost while capturing, stop capturing
            if (_isCapturing)
            {
                StopCapture();
            }
        }

        private void UpdateHotkeyText()
        {
            HotkeyText = Hotkey?.ToString() ?? "Click to set";
        }
    }
}
