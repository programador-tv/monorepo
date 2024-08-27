using Domain.Entities;

namespace Domain.WebServices
{
	public interface IVisualizationWebService
	{
		Task<List<Visualization>> GetLiveVisualizations(List<Guid> liveIds);
	}
}