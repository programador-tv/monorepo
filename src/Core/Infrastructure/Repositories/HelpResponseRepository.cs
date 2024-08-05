using Domain.Entities;
using Domain.Enumerables;
using Domain.Interfaces.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class HelpResponseRepository(ApplicationDbContext context)
    : GenericRepository<HelpResponse>(context),
        IHelpResponseRepository
{
    public async Task AddAsync(HelpResponse helpResponse)
    {
        await DbContext.HelpResponses.AddAsync(helpResponse);
        await DbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(HelpResponse helpResponse)
    {
        DbContext.HelpResponses.Update(helpResponse);
        await DbContext.SaveChangesAsync();
    }

    public async Task<List<HelpResponse>> GetAllAsync(Guid timeSelectionId) =>
        await DbContext
            .HelpResponses.Where(hlpr =>
                hlpr.ResponseStatus == ResponseStatus.Posted
                && hlpr.TimeSelectionId == timeSelectionId
            )
            .OrderByDescending(hlpr => hlpr.CreatedAt)
            .ToListAsync();

    public async Task<HelpResponse> GetById(Guid id) => await DbContext.HelpResponses.FindAsync(id);
}
