using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Logic;

public sealed class FollowBusinessLogic(IFollowRepository _repository) : IFollowBusinessLogic
{
    public async Task<ToggleFollowResponse> ToggleFollow(Guid followerId, Guid followingId)
    {
        var follow = await _repository.GetByIdAsync(followerId, followingId);

        if (follow == null)
        {
            var newFollow = Follow.Create(followerId, followingId);
            await _repository.Create(newFollow);
            return new ToggleFollowResponse(newFollow.Active);
        }
        else if (follow.Active)
        {
            follow.UnfollowUser();
        }
        else
        {
            follow.FollowUser();
        }

        await _repository.Update(follow);
        return new ToggleFollowResponse(follow.Active);
    }

    public async Task<FollowInformationResponse> GetFollowInformation(Guid userId)
    {
        var followers = await _repository.GetFollowersAsync(userId);
        var following = await _repository.GetFollowingAsync(userId);

        return new FollowInformationResponse(followers, following);
    }

    public async Task<List<FollowersResponse>> GetFollowersCount(List<Guid> usersId)
    {
        return await _repository.GetFollowersByIdsAsync(usersId);
    }

    public async Task<IsFollowingResponse> IsFollowing(Guid followerId, Guid followingId)
    {
        return await _repository.IsFollowingAsync(followerId, followingId);
    }
}
