using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class VisualizationRepository(ApplicationDbContext context)
        : GenericRepository<Visualization>(context),
            IVisualizationRepository
    {
        // Summary:
        //		Searches the Visualizations table for all visualizations that contain a LiveId present in the visibleLiveIds list.
        //	Returns:
        //		List of Visualization entity (can be empty).
        public async Task<List<Visualization>> GetVisualizationsByLiveIds(
            List<Guid> visibleLivesIds
        )
        {
            var liveVisualizations = await DbContext
                .Visualizations.Where(e => visibleLivesIds.Contains(e.LiveId))
                .ToListAsync();
            return liveVisualizations ?? [];
        }
    }
}
