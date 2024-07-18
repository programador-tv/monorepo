using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class HelpBackstageRepository(ApplicationDbContext context)
    : GenericRepository<Notification>(context),
        IHelpBackstageRepository
{
    public async Task AddAsync(HelpBackstage backstage)
    {
        await DbContext.HelpBackstages.AddAsync(backstage);
        await DbContext.SaveChangesAsync();
    }

    public async Task<HelpBackstage?> GetByTimeSelectionIdAsync(Guid id) =>
        await DbContext.HelpBackstages.FirstAsync(e => e.TimeSelectionId == id);

    public async Task UpdateAsync(HelpBackstage backstage)
    {
        DbContext.HelpBackstages.Update(backstage);
        await DbContext.SaveChangesAsync();
    }

    public async Task<List<HelpBackstage>> GetByIdsAsync(List<Guid> ids) =>
        await DbContext.HelpBackstages.Where(e => ids.Contains(e.TimeSelectionId)).ToListAsync();
}
