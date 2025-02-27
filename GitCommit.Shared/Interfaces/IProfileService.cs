using System.Collections.Generic;
using System.Threading.Tasks;
using GitCommit.Shared.Models;

namespace GitCommit.Shared.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileResponse> GetProfile(int userId);
        Task<List<User>> GetProfilesToExplore(int userId);
        Task<bool> UpdateProfile(User user);
        Task<bool> UpdatePreferences(int userId, UserPreferences preferences);
        Task<bool> AddProfileImage(int userId, UserImage image);
        Task<UserImage> GetProfileImage(int imageId);
    }
}
