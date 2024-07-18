using System;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PerfilRepository(ApplicationDbContext context)
    : GenericRepository<Perfil>(context),
        IPerfilRepository
{
    public async Task<List<Perfil>> GetAllAsync() => await DbContext.Perfils.ToListAsync();

    public async Task<Perfil?> GetByIdAsync(Guid id) => await DbContext.Perfils.FindAsync(id);

    public async Task<List<Perfil>> GetByIdsAsync(List<Guid> ids) =>
        await DbContext.Perfils.Where(e => ids.Contains(e.Id)).ToListAsync();

    public async Task<Perfil?> GetByTokenAsync(string token) =>
        await DbContext.Perfils.FirstOrDefaultAsync(e => e.Token == token);

    public async Task<Perfil?> GetByUsernameAsync(string username) =>
        await DbContext.Perfils.FirstOrDefaultAsync(e => e.UserName == username);

    public async Task<List<Perfil>> GetWhenContainsAsync(string keyword) =>
        await DbContext
            .Perfils.Where(e =>
                e.Nome.ToLower().Contains(keyword.ToLower())
                || e.UserName.ToLower().Contains(keyword.ToLower())
            )
            .ToListAsync();

    public async Task AddAsync(Perfil perfil)
    {
        await DbContext.Perfils.AddAsync(perfil);
        await DbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Perfil perfil)
    {
        DbContext.Perfils.Update(perfil);
        await DbContext.SaveChangesAsync();
    }
}
