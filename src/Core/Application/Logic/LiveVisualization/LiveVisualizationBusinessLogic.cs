using Domain.Entities;
using Domain.Repositories;

namespace Application.Logic
{
	public class LiveVisualizationBusinessLogic(ILiveVisualizationsRepository _repository) : ILiveVisualizationBusinessLogic
	{
		public async Task<List<Visualization>> GetLiveVisualizations(List<Guid> liveIds)
		{
			return await _repository.GetVisualizationsByLiveIds(liveIds);
		}
	}
}