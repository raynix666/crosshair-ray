
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Input;

namespace CrosshairApp.Models
{
    public class HotkeySettings : INotifyPropertyChanged
    {
        private KeyGesture? _toggleOverlayHotkey = new KeyGesture(Key.F8);
        private KeyGesture? _increaseSizeHotkey = new KeyGesture(Key.PageUp);
        private KeyGesture? _decreaseSizeHotkey = new KeyGesture(Key.PageDown);
        private KeyGesture? _changePresetHotkey = new KeyGesture(Key.Home);
        private KeyGesture? _toggleRgbHotkey = new KeyGesture(Key.End);
        private KeyGesture? _hideShowCrosshairHotkey = new KeyGesture(Key.Insert);

        public KeyGesture? ToggleOverlayHotkey
        {
            get => _toggleOverlayHotkey;
            set => SetProperty(ref _toggleOverlayHotkey, value);
        }

        public KeyGesture? IncreaseSizeHotkey
        {
            get => _increaseSizeHotkey;
            set => SetProperty(ref _increaseSizeHotkey, value);
        }

        public KeyGesture? DecreaseSizeHotkey
        {
            get => _decreaseSizeHotkey;
            set => SetProperty(ref _decreaseSizeHotkey, value);
        }

        public KeyGesture? ChangePresetHotkey
        {
            get => _changePresetHotkey;
            set => SetProperty(ref _changePresetHotkey, value);
        }

        public KeyGesture? ToggleRgbHotkey
        {
            get => _toggleRgbHotkey;
            set => SetProperty(ref _toggleRgbHotkey, value);
        }

        public KeyGesture? HideShowCrosshairHotkey
        {
            get => _hideShowCrosshairHotkey;
            set => SetProperty(ref _hideShowCrosshairHotkey, value);
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
}
