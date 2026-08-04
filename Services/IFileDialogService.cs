
using System.Threading.Tasks;

namespace CrosshairApp.Services
{
    public interface IFileDialogService
    {
        Task<string?> OpenFileAsync(string title, string filter);
        Task<string?> SaveFileAsync(string title, string defaultFileName, string filter);
    }
}
