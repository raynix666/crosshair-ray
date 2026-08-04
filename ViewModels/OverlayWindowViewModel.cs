
using CommunityToolkit.Mvvm.ComponentModel;
using CrosshairApp.Models;

namespace CrosshairApp.ViewModels
{
    public partial class OverlayWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private CrosshairSettings _crosshairSettings;

        public OverlayWindowViewModel(CrosshairSettings settings)
        {
            _crosshairSettings = settings;
        }
    }
}
