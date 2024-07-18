using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Logic;

public interface ILikeBusinessLogic
{
    public Task<List<Like>> GetLikesByLiveId(Guid liveId);
}
