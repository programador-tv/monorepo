using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Logic;

public interface IFollowBusinessLogic
{
    public Task<ToggleFollowResponse> ToggleFollow(Guid followerId, Guid followingId);
    public Task<FollowInformationResponse> GetFollowInformation(Guid userId);

    Task<List<FollowersResponse>> GetFollowersCount(List<Guid> usersId);
    public Task<bool> IsFollowing(Guid followerId, Guid followingId);
}
