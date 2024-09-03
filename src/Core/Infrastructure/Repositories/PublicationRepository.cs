using System;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PublicationRepository(ApplicationDbContext context)
    : GenericRepository<Publication>(context),
        IPublicationRepository
{

    public async Task AddAsync(Publication publication)
    {
        await DbContext.Publications.AddAsync(publication);
        await DbContext.SaveChangesAsync();
    }
}
