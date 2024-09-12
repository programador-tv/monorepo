using Domain.Contracts;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IFollowRepository
    {
        Task<Follow?> GetByIdAsync(Guid followerId, Guid followingId);

        Task Create(Follow follow);

        Task<bool> Update(Follow follow);

        Task<int> GetFollowersAsync(Guid userId);
        Task<int> GetFollowingAsync(Guid userId);

        Task<List<FollowersResponse>> GetFollowersByIdsAsync(List<Guid> usersId);
        Task<IsFollowingResponse> IsFollowingAsync(Guid followerId, Guid followingId);
    }
}
