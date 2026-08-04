
using CrosshairApp.Models;
using System.Threading.Tasks;

namespace CrosshairApp.Services
{
    public interface ISettingsService
    {
        Task<AppSettings> LoadSettingsAsync();
        Task SaveSettingsAsync(AppSettings settings);
    }
}
