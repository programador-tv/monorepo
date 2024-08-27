using Domain.Entities;

namespace Application.Logic
{
	public interface IVisualizationBusinessLogic
	{
		Task<List<Visualization>> GetLiveVisualizations(List<Guid> liveIds);
	}
}