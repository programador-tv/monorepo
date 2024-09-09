using System;
using Domain.Entities;
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
    public async Task<List<Publication>> GetAllAsync(Guid perfilId, int pageSize, int pageNumber) =>
        await DbContext
            .Publications.Where(e => e.PerfilId == perfilId)
            .OrderByDescending(e => e.PerfilId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task AddAsync(Publication publication)
    {
        await DbContext.Publications.AddAsync(publication);
        await DbContext.SaveChangesAsync();
    }
}
