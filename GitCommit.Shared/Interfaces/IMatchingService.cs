using System.Threading.Tasks;
using GitCommit.Shared.Models;

namespace GitCommit.Shared.Interfaces
{
    public interface IMatchingService
    {
        Task<bool> LikeProfile(LikeAction likeAction);
        Task<bool> DislikeProfile(DislikeAction dislikeAction);
        Task<MatchesResponse> GetMatches(int userId);
    }
}
