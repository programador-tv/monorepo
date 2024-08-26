using Domain.Entities;

namespace Domain.WebServices
{
	public interface ILiveVisualizationWebService
	{
		Task<List<Visualization>> GetLiveVisualizations(List<Guid> liveIds);
	}
}