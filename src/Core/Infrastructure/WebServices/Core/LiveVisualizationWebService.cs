using Domain.Entities;
using Domain.WebServices;

namespace Infrastructure.WebServices
{
	public sealed class LiveVisualizationWebService(CoreClient client) : ILiveVisualizationWebService
	{
		const string baseRoute = "api/livevisualization";

		public async Task<List<Visualization>> GetLiveVisualizations(List<Guid> liveIds)
		{
			var route = Path.Combine(baseRoute, String.Empty);
			return await client.PostAsync<List<Visualization>>(route, liveIds);
		}
	}
}