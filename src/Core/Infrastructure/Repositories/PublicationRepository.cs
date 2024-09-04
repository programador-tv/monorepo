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
    public async Task<List<Publication>> GetPublicationPerfilById(Guid perfilId)
    {
        return await DbContext.Publications.Where(e => e.PerfilId == perfilId).ToListAsync();
    }
}
