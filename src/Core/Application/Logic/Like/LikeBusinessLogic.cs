using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Logic;

public sealed class LikeBusinessLogic(ILikeRepository _repository) : ILikeBusinessLogic
{
    public async Task<List<Like>> GetLikesByLiveId(Guid liveId) =>
        await _repository.GetAllLikesByLiveId(liveId);

    public async Task CreateLike(CreateLikeRequest likeRequest)
    {
        var newLike = Like.Create(likeRequest.EntityId, likeRequest.RelatedUserId);
        await _repository.CreateLike(newLike);
    }
}
