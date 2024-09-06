using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PublicationRepository(ApplicationDbContext context)
    : GenericRepository<Publication>(context),
        IPublicationRepository
{
    public async Task<List<Publication>> GetAllAsync(Guid perfilId) =>
        await DbContext
            .Publications.Where(e => e.PerfilId == perfilId)
            .OrderByDescending(e => e.PerfilId)
            .ToListAsync();
}
