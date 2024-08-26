using Domain.Entities;

namespace Application.Logic
{
	public interface ILiveVisualizationBusinessLogic
	{
		Task<List<Visualization>> GetLiveVisualizations(List<Guid> liveIds);
	}
}