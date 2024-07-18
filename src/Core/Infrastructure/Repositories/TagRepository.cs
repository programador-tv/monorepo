using System;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class TagRepository(ApplicationDbContext context)
    : GenericRepository<Perfil>(context),
        ITagRepository
{
    public async Task<List<Tag>> GetAllByFreetimeIdAsync(string id) =>
        await DbContext.Tags.Where(e => e.FreeTimeRelacao == id).ToListAsync();

    public async Task<List<Tag>> GetTagRelationByKeyword(string keyword)
    {
        var tags = await DbContext
            .Tags.Where(tag =>
                keyword.ToUpper().Contains((tag.Titulo ?? "").ToUpper()) && tag.LiveRelacao != null
            )
            .ToListAsync();

        return tags;
    }
}
