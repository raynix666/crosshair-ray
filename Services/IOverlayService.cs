
using CrosshairApp.Models;
using System.Threading.Tasks;

namespace CrosshairApp.Services
{
    public interface IOverlayService
    {
        void ShowOverlay(CrosshairSettings settings);
        void HideOverlay();
        void CloseOverlay();
        void UpdateOverlaySettings(CrosshairSettings settings);
        void SetOverlayPosition(double x, double y, double width, double height);
        bool IsOverlayVisible { get; }
    }
}
