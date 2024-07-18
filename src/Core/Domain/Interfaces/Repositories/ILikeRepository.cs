using Domain.Entities;

namespace Domain.Repositories
{
    public interface ILikeRepository
    {
        public Task<List<Like>> GetAllLikesByLiveId(Guid liveId);
    }
}
