using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
	public class LiveVisualizationRepository(ApplicationDbContext context) : GenericRepository<Visualization>(context), ILiveVisualizationsRepository
	{
		public async Task<List<Visualization>> GetVisualizationsByLiveIds(List<Guid> visibleLivesIds)
		{
			var liveVisualizations = await DbContext.Visualizations
			.Where(e => visibleLivesIds.Contains(e.LiveId))
			.ToListAsync();
			return liveVisualizations ?? [];
		}
	}
}