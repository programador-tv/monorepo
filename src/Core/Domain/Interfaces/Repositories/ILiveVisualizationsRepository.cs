using Domain.Entities;

namespace Domain.Repositories
{
	public interface ILiveVisualizationsRepository
	{
		Task<List<Visualization>> GetVisualizationsByLiveIds(List<Guid> visibleLivesIds);
	}
}