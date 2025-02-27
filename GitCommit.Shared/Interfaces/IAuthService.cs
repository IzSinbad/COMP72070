using System.Threading.Tasks;
using GitCommit.Shared.Models;

namespace GitCommit.Shared.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<RegisterResponse> Register(RegisterRequest request);
        Task<bool> Logout(string token);
        Task<bool> UpdateUserStatus(int userId, UserStatus status);
    }
}
