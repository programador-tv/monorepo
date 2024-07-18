using System;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class LikeRepository(ApplicationDbContext context)
    : GenericRepository<Like>(context),
        ILikeRepository
{
    public async Task<List<Like>> GetAllLikesByLiveId(Guid liveId)
    {
        return await DbContext.Likes.Where(e => e.EntityId == liveId).ToListAsync();
    }
}
