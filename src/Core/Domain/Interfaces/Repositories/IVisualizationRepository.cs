using Domain.Entities;

namespace Domain.Repositories
{
    public interface IVisualizationRepository
    {
        Task<List<Visualization>> GetVisualizationsByLiveIds(List<Guid> visibleLivesIds);
    }
}
