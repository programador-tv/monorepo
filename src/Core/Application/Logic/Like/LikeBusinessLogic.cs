using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Logic;

public sealed class LikeBusinessLogic(ILikeRepository _repository) : ILikeBusinessLogic
{
    public async Task<List<Like>> GetLikesByLiveId(Guid liveId) =>
        await _repository.GetAllLikesByLiveId(liveId);
}
