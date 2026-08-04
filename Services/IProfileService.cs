
using CrosshairApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrosshairApp.Services
{
    public interface IProfileService
    {
        Task<List<GameProfile>> LoadProfilesAsync();
        Task SaveProfilesAsync(List<GameProfile> profiles);
        Task AddProfileAsync(GameProfile profile);
        Task UpdateProfileAsync(GameProfile profile);
        Task DeleteProfileAsync(GameProfile profile);
        Task<GameProfile?> GetActiveProfileAsync();
    }
}
