using Domain.Entities;
using Domain.Repositories;

namespace Application.Logic
{
	public class VisualizationBusinessLogic(IVisualizationRepository _repository) : IVisualizationBusinessLogic
	{
		public async Task<List<Visualization>> GetLiveVisualizations(List<Guid> liveIds)
		{
			return await _repository.GetVisualizationsByLiveIds(liveIds);
		}
	}
}